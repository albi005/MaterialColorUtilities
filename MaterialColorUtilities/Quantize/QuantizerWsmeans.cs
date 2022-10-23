// Copyright 2021 Google LLC
// Copyright 2021-2022 project contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#nullable disable

namespace MaterialColorUtilities.Quantize;

/// <summary>
/// An image quantizer that improves on the speed of a standard K-Means algorithm by implementing
/// several optimizations, including deduping identical pixels and a triangle inequality rule that
/// reduces the number of comparisons needed to identify which cluster a point should be moved to.
/// </summary>
/// <remarks>
/// Wsmeans stands for Weighted Square Means.<para/>
/// This algorithm was designed by M. Emre Celebi, and was found in their 2011 paper,
/// Improving the Performance of K-Means for Color Quantization.
/// <see href="https://arxiv.org/abs/1101.0395"/>
/// </remarks>
public class QuantizerWsmeans
{
    private class DistanceAndIndex : IComparable<DistanceAndIndex>
    {
        public int Index { get; set; } = -1;
        public double Distance { get; set; } = -1;

        public int CompareTo(DistanceAndIndex other) => Distance.CompareTo(other.Distance);
    }

    private const uint MAX_ITERATIONS = 10;
    private const double MIN_MOVEMENT_DISTANCE = 3.0;

    /// <summary>
    /// Reduce the number of colors needed to represented the input, minimizing the
    /// difference between the original image and the recolored image.
    /// </summary>
    /// <param name="inputPixels">Colors in ARGB format.</param>
    /// <param name="startingClusters">
    /// Defines the initial state of the quantizer. Passing an empty
    /// array is fine, the implementation will create its own initial state that leads to
    /// reproducible results for the same inputs. Passing an array that is the result of Wu
    /// quantization leads to higher quality results.
    /// </param>
    /// <param name="maxColors">
    /// The number of colors to divide the image into. A lower number of colors may be returned.
    /// </param>
    /// <returns>
    /// A dictionary with keys of colors in ARGB format, values of how many of the input pixels belong
    /// to the color.
    /// </returns>
    public static Dictionary<uint, uint> Quantize(
        uint[] inputPixels, uint[] startingClusters, uint maxColors)
    {
        Dictionary<uint, uint> pixelToCount = new();
        List<double[]> points = new(); // Point index to Lab representation.
        List<uint> pixels = new(); // Point index to RGB representation.
        IPointProvider pointProvider = new PointProviderLab();

        uint pointCount = 0; // The number of unique colors found in inputPixels.
        for (uint i = 0; i < inputPixels.Length; i++)
        {
            uint inputPixel = inputPixels[i];
            if (pixelToCount.TryGetValue(inputPixel, out uint pixelCount))
            {
                pixelToCount[inputPixel] = pixelCount + 1;
            }
            else
            {
                // A new unique color has been found.
                // Calculate its LAB representation and cache it.
                points.Add(pointProvider.FromInt(inputPixel));
                // Store the ARGB value at the same index.
                // The index will be the number of unique colors found before this color.
                pixels.Add(inputPixel);
                pointCount++;

                pixelToCount[inputPixel] = 1;
            }
        }

        uint[] counts = new uint[pointCount]; // Point index to how many times it occures in inputPixels.
        for (int i = 0; i < pointCount; i++)
        {
            uint pixel = pixels[i];
            uint count = pixelToCount[pixel];
            counts[i] = count;
        }

        uint clusterCount = Math.Min(maxColors, pointCount); // The number of clusters to create.
        if (startingClusters.Length != 0) // If starting clusters have been specified, use those.
        {
            clusterCount = Math.Min(clusterCount, (uint)startingClusters.Length);
        }

        double[][] clusters = new double[clusterCount][]; // A cluster is a point in Lab space. Every color will be grouped/assigned to one.
        uint clustersCreated = 0;
        // Create clusters from startingClusters.
        for (uint i = 0; i < startingClusters.Length; i++)
        {
            clusters[i] = pointProvider.FromInt(startingClusters[i]);
            clustersCreated++;
        }

        // If startingClusters is empty...
        // ...in Dart choose randomly from the input colors.
        // ...in TypeScript create random Lab points.
        // ...in Java do nothing, which will cause an exception later.
        // This can happen when the inputs are not a result of Wu quantization.
        int additionalClustersNeeded = (int)clusterCount - (int)clustersCreated;
        if (additionalClustersNeeded > 0)
        {
            // Use existing points rather than generating random centroids.
            //
            // KMeans is extremely sensitive to initial clusters. This quantizer
            // is meant to be used with a Wu quantizer that provides initial
            // centroids, but Wu is very slow on unscaled images and when extracting
            // more than 256 colors.
            //
            // Here, we can safely assume that more than 256 colors were requested
            // for extraction. Generating random centroids tends to lead to many
            // "empty" centroids, as the random centroids are nowhere near any pixels
            // in the image, and the centroids from Wu are very refined and close
            // to pixels in the image.
            //
            // Rather than generate random centroids, we'll pick centroids that
            // are actual pixels in the image, and avoid duplicating centroids.

            Random random = new();
            List<int> usedIndices = new(additionalClustersNeeded);
            for (uint i = 0; i < additionalClustersNeeded; i++)
            {
                int index = random.Next(points.Count);
                while (usedIndices.Contains(index))
                {
                    index = random.Next(points.Count);
                }

                usedIndices.Add(index);
                clusters[clustersCreated + i] = points[index];
            }
        }

        // Assign every point to a random cluster
        uint[] clusterIndices = new uint[pointCount]; // Point index to containing cluster's index
        for (uint i = 0; i < pointCount; i++)
        {
            // This is enough as clusters are already randomly ordered.
            clusterIndices[i] = i % clusterCount;
        }

        // Initialize distanceToIndexMatrix, which will store the distance between clusters
        DistanceAndIndex[][] distanceToIndexMatrix = new DistanceAndIndex[clusterCount][];
        for (uint i = 0; i < clusterCount; i++)
        {
            distanceToIndexMatrix[i] = new DistanceAndIndex[clusterCount];
            for (uint j = 0; j < clusterCount; j++)
            {
                distanceToIndexMatrix[i][j] = new DistanceAndIndex();
            }
        }

        // Start iterating
        uint[] pixelCountSums = new uint[clusterCount]; // The number of pixels per cluster
        for (uint iteration = 0; iteration < MAX_ITERATIONS; iteration++)
        {
            // For every cluster...
            for (int i = 0; i < clusterCount; i++)
            {
                // ...cache the distance to every other cluster.
                for (int j = i + 1; j < clusterCount; j++)
                {
                    double distance = pointProvider.Distance(clusters[i], clusters[j]);
                    distanceToIndexMatrix[j][i].Distance = distance;
                    distanceToIndexMatrix[j][i].Index = i;
                    distanceToIndexMatrix[i][j].Distance = distance;
                    distanceToIndexMatrix[i][j].Index = j;
                }

                // Sort the distances to the other clusters.
                // This way, distanceToIndexMatrix[i][0] will be the closest cluster's index (a color ID) and distance.
                Array.Sort(distanceToIndexMatrix[i]);
            }

            // Assign every point to the closest cluster
            uint pointsMoved = 0;
            for (int i = 0; i < pointCount; i++)
            {
                double[] point = points[i];
                uint previousClusterIndex = clusterIndices[i];
                double[] previousCluster = clusters[previousClusterIndex];
                double previousDistance = pointProvider.Distance(point, previousCluster);

                double minimumDistance = previousDistance;
                int newClusterIndex = -1;
                // Find the cluster closest to the point
                for (int j = 0; j < clusterCount; j++)
                {
                    // Let A be the point for which we need the closest cluster,
                    // let B be previous cluster the point was assigned to and
                    // let C be the current cluster in the enumeration.
                    // If BC >= 2AB, then AC can't be < AB (triangle inequality rule).

                    // 4 is used here instead of 2,
                    // as pointProvider returns the square of the actual distance to save time.
                    if (distanceToIndexMatrix[previousClusterIndex][j].Distance >= 4 * previousDistance)
                    {
                        continue;
                    }
                    double distance = pointProvider.Distance(point, clusters[j]);
                    if (distance < minimumDistance)
                    {
                        minimumDistance = distance;
                        newClusterIndex = j;
                    }
                }
                if (newClusterIndex != -1)
                {
                    double distanceChange =
                        Math.Abs(Math.Sqrt(minimumDistance) - Math.Sqrt(previousDistance));
                    if (distanceChange > MIN_MOVEMENT_DISTANCE)
                    {
                        pointsMoved++;
                        // Set the point's cluster to the closest one we found
                        clusterIndices[i] = (uint)newClusterIndex;
                    }
                }
            }

            if (pointsMoved == 0 && iteration != 0)
            {
                break;
            }

            // For every cluster, individually sum the LAB components...
            double[] componentASums = new double[clusterCount];
            double[] componentBSums = new double[clusterCount];
            double[] componentCSums = new double[clusterCount];
            for (int i = 0; i < pixelCountSums.Length; i++)
                pixelCountSums[i] = 0;
            for (int i = 0; i < pointCount; i++)
            {
                uint clusterIndex = clusterIndices[i];
                double[] point = points[i];
                uint count = counts[i];
                pixelCountSums[clusterIndex] += count;
                componentASums[clusterIndex] += point[0] * count;
                componentBSums[clusterIndex] += point[1] * count;
                componentCSums[clusterIndex] += point[2] * count;
            }

            // ...then set the cluster's position to the average position.
            for (int i = 0; i < clusterCount; i++)
            {
                uint count = pixelCountSums[i];
                if (count == 0)
                {
                    clusters[i] = new double[] { 0, 0, 0 };
                    continue;
                }
                double a = componentASums[i] / count;
                double b = componentBSums[i] / count;
                double c = componentCSums[i] / count;
                clusters[i][0] = a;
                clusters[i][1] = b;
                clusters[i][2] = c;
            }
        }

        Dictionary<uint, uint> argbToPopulation = new();
        for (uint i = 0; i < clusterCount; i++)
        {
            uint count = pixelCountSums[i];
            if (count == 0)
            {
                continue;
            }

            uint possibleNewCluster = pointProvider.ToInt(clusters[i]);
            if (argbToPopulation.ContainsKey(possibleNewCluster))
            {
                continue;
            }

            argbToPopulation[possibleNewCluster] = count;
        }

        return argbToPopulation;
    }
}
