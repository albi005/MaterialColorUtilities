namespace MaterialColorUtilities.Quantize;

/**
 * An image quantizer that improves on the speed of a standard K-Means algorithm by implementing
 * several optimizations, including deduping identical pixels and a triangle inequality rule that
 * reduces the number of comparisons needed to identify which cluster a point should be moved to.
 *
 * <p>Wsmeans stands for Weighted Square Means.
 *
 * <p>This algorithm was designed by M. Emre Celebi, and was found in their 2011 paper, Improving
 * the Performance of K-Means for Color Quantization. https://arxiv.org/abs/1101.0395
 */
public class QuantizerWsmeans
{
    private class Distance : IComparable<Distance>
    {
        public int Index { get; set; } = -1;
        public double Distance1 { get; set; } = -1; //TODO: Rename to Value

        public int CompareTo(Distance other) => Distance1.CompareTo(other.Distance1);
    }

    private const int MAX_ITERATIONS = 10;
    private const double MIN_MOVEMENT_DISTANCE = 3.0;

    /**
     * Reduce the number of colors needed to represented the input, minimizing the difference between
     * the original image and the recolored image.
     *
     * @param inputPixels Colors in ARGB format.
     * @param startingClusters Defines the initial state of the quantizer. Passing an empty array is
     *     fine, the implementation will create its own initial state that leads to reproducible
     *     results for the same inputs. Passing an array that is the result of Wu quantization leads
     *     to higher quality results.
     * @param maxColors The number of colors to divide the image into. A lower number of colors may be
     *     returned.
     * @return Map with keys of colors in ARGB format, values of how many of the input pixels belong
     *     to the color.
     */
    public static Dictionary<int, int> Quantize(
        int[] inputPixels, int[] startingClusters, int maxColors)
    {
        Dictionary<int, int> pixelToCount = new();
        double[][] points = new double[inputPixels.Length][];
        int[] pixels = new int[inputPixels.Length];
        IPointProvider pointProvider = new PointProviderLab();

        int pointCount = 0;
        for (int i = 0; i < inputPixels.Length; i++)
        {
            int inputPixel = inputPixels[i];
            if (pixelToCount.TryGetValue(inputPixel, out int pixelCount))
            {
                pixelToCount[inputPixel] = pixelCount + 1;
            }
            else
            {
                points[pointCount] = pointProvider.FromInt(inputPixel);
                pixels[pointCount] = inputPixel;
                pointCount++;

                pixelToCount[inputPixel] = 1;
            }
        }

        int[] counts = new int[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            int pixel = pixels[i];
            int count = pixelToCount[pixel];
            counts[i] = count;
        }

        int clusterCount = Math.Min(maxColors, pointCount);
        if (startingClusters.Length != 0)
        {
            clusterCount = Math.Min(clusterCount, startingClusters.Length);
        }

        double[][] clusters = new double[clusterCount][];
        int clustersCreated = 0;
        for (int i = 0; i < startingClusters.Length; i++)
        {
            clusters[i] = pointProvider.FromInt(startingClusters[i]);
            clustersCreated++;
        }

        int additionalClustersNeeded = clusterCount - clustersCreated;
        if (additionalClustersNeeded > 0)
        {
            for (int i = 0; i < additionalClustersNeeded; i++) { }
        }

        Random random = new();

        int[] clusterIndices = new int[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            clusterIndices[i] = (int)Math.Floor(random.NextDouble() * clusterCount);
        }

        int[][] indexMatrix = new int[clusterCount][];
        for (int i = 0; i < clusterCount; i++)
        {
            indexMatrix[i] = new int[clusterCount];
        }

        Distance[][] distanceToIndexMatrix = new Distance[clusterCount][];
        for (int i = 0; i < clusterCount; i++)
        {
            distanceToIndexMatrix[i] = new Distance[clusterCount];
            for (int j = 0; j < clusterCount; j++)
            {
                distanceToIndexMatrix[i][j] = new Distance();
            }
        }

        int[] pixelCountSums = new int[clusterCount];
        for (int iteration = 0; iteration < MAX_ITERATIONS; iteration++)
        {
            for (int i = 0; i < clusterCount; i++)
            {
                for (int j = i + 1; j < clusterCount; j++)
                {
                    double distance = pointProvider.Distance(clusters[i], clusters[j]);
                    distanceToIndexMatrix[j][i].Distance1 = distance;
                    distanceToIndexMatrix[j][i].Index = i;
                    distanceToIndexMatrix[i][j].Distance1 = distance;
                    distanceToIndexMatrix[i][j].Index = j;
                }
                Array.Sort(distanceToIndexMatrix[i]);
                for (int j = 0; j < clusterCount; j++)
                {
                    indexMatrix[i][j] = distanceToIndexMatrix[i][j].Index;
                }
            }

            int pointsMoved = 0;
            for (int i = 0; i < pointCount; i++)
            {
                double[] point = points[i];
                int previousClusterIndex = clusterIndices[i];
                double[] previousCluster = clusters[previousClusterIndex];
                double previousDistance = pointProvider.Distance(point, previousCluster);

                double minimumDistance = previousDistance;
                int newClusterIndex = -1;
                for (int j = 0; j < clusterCount; j++)
                {
                    if (distanceToIndexMatrix[previousClusterIndex][j].Distance1 >= 4 * previousDistance)
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
                        clusterIndices[i] = newClusterIndex;
                    }
                }
            }

            if (pointsMoved == 0 && iteration != 0)
            {
                break;
            }

            double[] componentASums = new double[clusterCount];
            double[] componentBSums = new double[clusterCount];
            double[] componentCSums = new double[clusterCount];
            for (int i = 0; i < pixelCountSums.Length; i++) pixelCountSums[i] = 0;
            for (int i = 0; i < pointCount; i++)
            {
                int clusterIndex = clusterIndices[i];
                double[] point = points[i];
                int count = counts[i];
                pixelCountSums[clusterIndex] += count;
                componentASums[clusterIndex] += (point[0] * count);
                componentBSums[clusterIndex] += (point[1] * count);
                componentCSums[clusterIndex] += (point[2] * count);
            }

            for (int i = 0; i < clusterCount; i++)
            {
                int count = pixelCountSums[i];
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

        Dictionary<int, int> argbToPopulation = new();
        for (int i = 0; i < clusterCount; i++)
        {
            int count = pixelCountSums[i];
            if (count == 0)
            {
                continue;
            }

            int possibleNewCluster = pointProvider.ToInt(clusters[i]);
            if (argbToPopulation.ContainsKey(possibleNewCluster))
            {
                continue;
            }

            argbToPopulation[possibleNewCluster] = count;
        }

        return argbToPopulation;
    }
}
