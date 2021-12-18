#nullable disable

namespace Monet;

public static class Utils
{
    public static double Linearized(double rgb) => .04045 >= rgb ? rgb / 12.92 : Math.Pow((rgb + .055) / 1.055, 2.4);
    public static double Delinearized(double rgb) => .0031308 >= rgb ? 12.92 * rgb : 1.055 * Math.Pow(rgb, 1 / 2.4) - .055;

    public static double LStarFromInt(uint argb)
    {
        double y = 21.26 * Linearized((double)((argb & 16711680) >> 16) / 255) + 71.52 * Linearized((double)((argb & 65280) >> 8) / 255) + 7.22 * Linearized((double)(argb & 255) / 255);
        y /= 100;
        return y <= (double)216 / 24389 ? (double)24389 / 27 * y : 116 * Math.Pow(y, (double)1 / 3) - 16;
    }

    public static uint IntFromRgb(byte[] rgb) =>
        0xFF000000 |
        ((uint)rgb[0] & 255) << 16 |
        ((uint)rgb[1] & 255) << 8 |
        (uint)rgb[2] & 255;

    public static uint IntFromXyzComponents(double x, double y, double z)
    {
        x /= 100;
        y /= 100;
        z /= 100;
        return IntFromRgb(new byte[]
        {
            (byte)Math.Round(Math.Clamp(255 * Delinearized(3.2406 * x + -1.5372 * y + -.4986 * z), 0, 255)),
            (byte)Math.Round(Math.Clamp(255 * Delinearized(-.9689 * x + 1.8758 * y + .0415 * z), 0, 255)),
            (byte)Math.Round(Math.Clamp(255 * Delinearized(.0557 * x + -.204 * y + 1.057 * z), 0, 255))
        });
    }

    public static uint IntFromLStar(double lStar)
    {
        double fy = (double)(lStar + 16) / 116;
        double kappa = (double)24389 / 27;
        bool cubeExceedEpsilon = fy * fy * fy > 216D / 24389;
        var xyz = new double[]
        {
            (cubeExceedEpsilon ? fy * fy * fy : (116 * fy - 16) / kappa) * Constants.WhitePointD65[0],
            (8 < lStar ? fy * fy * fy : lStar / kappa) * Constants.WhitePointD65[1],
            (cubeExceedEpsilon ? fy * fy * fy : (116 * fy - 16) / kappa) * Constants.WhitePointD65[2]
        };
        return IntFromXyzComponents(xyz[0], xyz[1], xyz[2]);
    }

    public static double SanitizeDegrees(double degrees)
        => 0 > degrees ? degrees % 360 + 360 : 360 <= degrees ? degrees % 360 : degrees;

    public static double[] LabFromInt(uint argb)
    {
        const double e = (double)216 / 24389;
        const double kappa = (double)24389 / 27;
        double redL = 100 * Linearized((double)((argb & 16711680) >> 16) / 255);
        double greenL = 100 * Linearized((double)((argb & 65280) >> 8) / 255);
        double blueL = 100 * Linearized((double)(argb & 255) / 255);
        double yNormalized = (.2126 * redL + .7152 * greenL + .0722 * blueL) / Constants.WhitePointD65[1];
        double fy = yNormalized > e ? Math.Pow(yNormalized, (double)1 / 3) : (kappa * yNormalized + 16) / 116;
        double xNormalized = (.41233895 * redL + .35762064 * greenL + .18051042 * blueL) / Constants.WhitePointD65[0];
        double zNormalized = (.01932141 * redL + .11916382 * greenL + .95034478 * blueL) / Constants.WhitePointD65[2];
        return new double[]
        {
            116 * fy - 16,
            500 * ((xNormalized > e ? Math.Pow(xNormalized, (double)1 / 3) : (kappa * xNormalized + 16) / 116) - fy),
            200 * (fy - (zNormalized > e ? Math.Pow(zNormalized, (double)1 / 3) : (kappa * zNormalized + 16) / 116))
        };
    }

    public class LabPointProvider
    {
        public uint ToInt(double[] point)
        {
            double l = point[0];
            double e = (double)216 / 24389;
            double kappa = (double)24389 / 27;
            double fy = (l + 16) / 116;
            double fx = point[1] / 500 + fy;
            double fz = fy - point[2] / 200;
            double fx3 = fx * fx * fx;
            double fz3 = fz * fz * fz;
            double[] xyz = new double[]
            {
                (fx3 > e ? fx3 : (116 * fx - 16) / kappa) * Constants.WhitePointD65[0],
                (l > 8 ? fy*fy*fy : l / kappa) * Constants.WhitePointD65[1],
                (fz3 > e ? fz3 : (116 * fz - 16) / kappa) * Constants.WhitePointD65[2]
            };
            return IntFromXyzComponents(xyz[0], xyz[1], xyz[2]);
        }
        public double Distance(double[] from, double[] to)
        {
            double dL = from[0] - to[0];
            double dA = from[1] - to[1];
            double dB = from[2] - to[2];
            return dL * dL + dA * dA + dB * dB;
        }
    }

    public class DistanceAndIndex
    {
        public double Distance { get; set; } = -1;
        public int Index { get; set; } = -1;
    }

    public static List<uint> Filter(Dictionary<uint, double> colorsToExcitedProportion, Dictionary<uint, Cam16> colorsToCam)
    {
        List<uint> filtered = new();
        foreach (var colorAndCam in colorsToCam)
        {
            var color = colorAndCam.Key;
            var cam = colorAndCam.Value;
            var proportion = colorsToExcitedProportion[color];
            if (15 <= cam.Chroma && 10 <= LStarFromInt(color) && proportion >= 0.1)
            {
                filtered.Add(color);
            }
        }
        return filtered;
    }

    public static List<uint> Score(Dictionary<uint, uint> colorsToPopulation)
    {
        uint populationSum = 0;
        foreach (var population in colorsToPopulation.Values)
            populationSum += population;
        Dictionary<uint, double> colorsToProportion = new();
        Dictionary<uint, Cam16> colorsToCam = new();
        double[] hueProportions = new double[360];
        foreach (var colorAndPopulation in colorsToPopulation)
        {
            uint color = colorAndPopulation.Key;
            double proportion = (double)colorAndPopulation.Value / populationSum;
            colorsToProportion[color] = proportion;
            Cam16 cam = Cam16.FromIntInViewingConditions(color);
            colorsToCam[color] = cam;
            hueProportions[(int)Math.Round(cam.Hue)] += proportion;
        }
        Dictionary<uint, double> colorsToExcitedProportion = new();
        foreach (var colorAndCam in colorsToCam)
        {
            uint color = colorAndCam.Key;
            uint hue = (uint)Math.Round(colorsToCam[color].Hue);
            double excitedProportion = 0;
            for (uint i = hue - 15; i < hue + 15; i++)
                excitedProportion += hueProportions[(int)SanitizeDegrees(i)];
            colorsToExcitedProportion[color] = excitedProportion;
        }

        var colorsToScore = new Dictionary<uint, double>();
        foreach (var colorAndCam in colorsToCam)
        {
            uint color = colorAndCam.Key;
            Cam16 cam = colorAndCam.Value;
            double proportionScore = 70 * colorsToExcitedProportion[color];
            colorsToScore[color] = proportionScore + (cam.Chroma - 48) * (48 > cam.Chroma ? .1 : .3);
        }

        List<uint> filteredColors = Filter(colorsToExcitedProportion, colorsToCam);
        Dictionary<uint, double> dedupedColorsToScore = new Dictionary<uint, double>();
        foreach (var color in filteredColors)
        {
            bool duplicateHue = false;
            var hue = colorsToCam[color].Hue;
            foreach (var colorAndScore in dedupedColorsToScore)
            {
                var alreadyChosenHue = colorsToCam[colorAndScore.Key].Hue;
                if (15 > 180 - Math.Abs(Math.Abs(hue - alreadyChosenHue) - 180))
                {
                    duplicateHue = true;
                    break;
                }
            }
            if (!duplicateHue)
                dedupedColorsToScore[color] = colorsToScore[color];
        }

        var answer = dedupedColorsToScore.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        if (answer.Count == 0) answer.Add(0xFF4285F4);
        return answer;
    }

    /// <summary>
    /// Compute a primary color from a collection of colors
    /// </summary>
    public static uint SeedFromImage(IEnumerable<uint> pixels)
    {
        QuantizerWu quantizer = new()
        {
            weights = new uint[35937],
            momentsR = new uint[35937],
            momentsG = new uint[35937],
            momentsB = new uint[35937],
            moments = new uint[35937]
        };
        Dictionary<uint, uint> countByColor = new();
        foreach (var pixel in pixels)
        {
            if (255 == ((pixel & 0xFF00_0000) >> 24))
            {
                if (!countByColor.ContainsKey(pixel))
                    countByColor[pixel] = 0;
                countByColor[pixel]++;
            }
        }

        foreach (var entry in countByColor)
        {
            uint pixel = entry.Key;
            uint count = entry.Value;
            uint red = (pixel & 0xFF0000) >> 16;
            uint green = (pixel & 0xFF00) >> 8;
            uint blue = pixel & 0xFF;
            uint index = GetIndex((red >> 3) + 1, (green >> 3) + 1, (blue >> 3) + 1);
            quantizer.weights[index] += count;
            quantizer.momentsR[index] += count * red;
            quantizer.momentsG[index] += count * green;
            quantizer.momentsB[index] += count * blue;
            quantizer.moments[index] += count * (red * red + green * green + blue * blue);
        }

        for (uint r = 1; r < 33; r++)
        {
            uint[] area = new uint[33];
            uint[] areaR = new uint[33];
            uint[] areaG = new uint[33];
            uint[] areaB = new uint[33];
            uint[] area2 = new uint[33];

            for (uint g = 1; g < 33; g++)
            {
                uint line = 0;
                uint lineR = 0;
                uint lineG = 0;
                uint lineB = 0;
                uint line2 = 0;
                for (uint b = 1; b < 33; b++)
                {
                    uint index = GetIndex(r, g, b);
                    line += quantizer.weights[index];
                    lineR += quantizer.momentsR[index];
                    lineG += quantizer.momentsG[index];
                    lineB += quantizer.momentsB[index];
                    line2 += quantizer.moments[index];
                    area[b] += line;
                    areaR[b] += lineR;
                    areaG[b] += lineG;
                    areaB[b] += lineB;
                    area2[b] += line2;
                    uint previousIndex = GetIndex(r - 1, g, b);
                    quantizer.weights[index] = quantizer.weights[previousIndex] + area[b];
                    quantizer.momentsR[index] = quantizer.momentsR[previousIndex] + areaR[b];
                    quantizer.momentsG[index] = quantizer.momentsG[previousIndex] + areaG[b];
                    quantizer.momentsB[index] = quantizer.momentsB[previousIndex] + areaB[b];
                    quantizer.moments[index] = quantizer.moments[previousIndex] + area2[b];
                }
            }
        }

        uint colorCount = quantizer.CreateBoxes().ResultCount;

        List<uint> colors = new();
        for (uint i = 0; i < colorCount; i++)
        {
            Box cube = quantizer.cubes[i];
            var weight = quantizer.Volume(cube, quantizer.weights);
            if (weight > 0)
            {
                uint rInner = (uint)Math.Round((double)quantizer.Volume(cube, quantizer.momentsR) / weight);
                uint gInner = (uint)Math.Round((double)quantizer.Volume(cube, quantizer.momentsG) / weight);
                uint bInner = (uint)Math.Round((double)quantizer.Volume(cube, quantizer.momentsB) / weight);
                colors.Add((uint)(-16777216 | (rInner & 255) << 16 | (gInner & 255) << 8 | bInner & 255));
            }
        }


        var pixelToCount = new Dictionary<uint, uint>();
        var points = new List<double[]>();
        var pixels0 = new List<uint>();
        LabPointProvider pointProvider = new();
        uint pointCount = 0;
        foreach (var inputPixel in pixels)
        {
            if (!pixelToCount.TryGetValue(inputPixel, out uint pixelCount))
            {
                pointCount++;
                points.Add(LabFromInt(inputPixel));
                pixels0.Add(inputPixel);
                pixelToCount[inputPixel] = 1;
            }
            else
            {
                pixelToCount[inputPixel] = pixelCount + 1;
            }
        }
        var counts = new Dictionary<uint, uint>();
        for (uint i = 0; i < pointCount; i++)
        {
            var count = pixelToCount[pixels0[(int)i]];
            if (count != 0)
                counts[i] = count;
        }
        uint clusterCount = Math.Min(256, pointCount);
        if (colors.Any()) clusterCount = Math.Min((uint)colors.Count, clusterCount);
        List<double[]> clusters = new();
        for (int i = 0; i < colors.Count; i++)
        {
            clusters.Add(LabFromInt(colors[i]));
        }
        long additionalClustersNeeded = clusterCount - colors.Count;
        Random random = new();
        if (colors.Count == 0 && additionalClustersNeeded > 0)
        {
            for (uint i = 0; i < additionalClustersNeeded; i++)
            {
                clusters.Add(new double[]
                {
                        100 * random.NextDouble(),
                        201 * random.NextDouble() + -100,
                        201 * random.NextDouble() + -100
                });
            }
        }
        List<uint> clusterIndices = new();
        for (uint i = 0; i < pointCount; i++)
        {
            clusterIndices.Add((uint)(random.NextDouble() * clusterCount));
        }
        var indexMatrix = new double[clusterCount, clusterCount]; // should only contain 0s
        var distanceToIndexMatrix = new DistanceAndIndex[clusterCount][];
        for (int i = 0; i < clusterCount; i++)
        {
            distanceToIndexMatrix[i] = new DistanceAndIndex[clusterCount];
            for (int j = 0; j < clusterCount; j++)
            {
                distanceToIndexMatrix[i][j] = new();
            }
        }

        var pixelCountSums = new uint[clusterCount];
        for (uint iteration = 0; iteration < 10; iteration++)
        {
            for (int i = 0; i < clusterCount; i++)
            {
                for (int j = i + 1; j < clusterCount; j++)
                {
                    double distance = pointProvider.Distance(clusters[i], clusters[j]);
                    distanceToIndexMatrix[j][i].Distance = distance;
                    distanceToIndexMatrix[j][i].Index = j;
                    distanceToIndexMatrix[i][j].Distance = distance;
                    distanceToIndexMatrix[i][j].Index = i;
                }
                Array.Sort(distanceToIndexMatrix[i], (x,y) 
                    => x.Index < y.Index 
                        ? -1 
                        : x.Index > y.Index 
                            ? 1
                            : 0
                            );
                for (uint j = 0; j < clusterCount; j++)
                {
                    indexMatrix[i, j] = distanceToIndexMatrix[i][j].Index;
                }
            }

            uint pointsMoved = 0;
            for (uint i = 0; i < pointCount; i++)
            {
                double[] point = points[(int)i];
                var previousClusterIndex = clusterIndices[(int)i];
                double previousDistance = pointProvider.Distance(point, clusters[(int)previousClusterIndex]);
                double minimumDistance = previousDistance;
                int newClusterindex = -1;
                for (uint j = 0; j < clusterCount; j++)
                {
                    if (distanceToIndexMatrix[previousClusterIndex][j].Distance >= 4 * previousDistance)
                        continue;
                    double distance = pointProvider.Distance(point, clusters[(int)j]);
                    if (distance < minimumDistance)
                    {
                        minimumDistance = distance;
                        newClusterindex = (int)j;
                    }
                }
                if (newClusterindex != -1 && 3 < Math.Abs(Math.Sqrt(minimumDistance) - Math.Sqrt(previousDistance)))
                {
                    pointsMoved++;
                    clusterIndices[(int)i] = (uint)newClusterindex;
                }
            }
            if (pointsMoved == 0 && iteration != 0)
                break;

            double[] componentASums = new double[clusterCount];
            double[] componentBSums = new double[clusterCount];
            double[] componentCSums = new double[clusterCount];
            for (uint i = 0; i < clusterCount; i++)
            {
                pixelCountSums[i] = 0;
            }
            for (uint i = 0; i < pointCount; i++)
            {
                uint clusterIndex = clusterIndices[(int)i];
                double[] point = points[(int)i];
                uint count = counts[i];
                pixelCountSums[(int)clusterIndex] += count;
                componentASums[(int)clusterIndex] += count * point[0];
                componentBSums[(int)clusterIndex] += count * point[1];
                componentCSums[(int)clusterIndex] += count * point[2];
            }
            for (int i = 0; i < clusterCount; i++)
            {
                uint count = pixelCountSums[i];
                clusters[i] = count == 0
                    ? new double[] { 0, 0, 0 }
                    : new double[] { componentASums[i] / count, componentBSums[i] / count, componentCSums[i] / count };
            }
        }

        Dictionary<uint, uint> argbToPopulation = new();
        for (int i = 0; i < clusterCount; i++)
        {
            uint count = pixelCountSums[i];
            if (count == 0)
                continue;
            uint possibleNewCluster = pointProvider.ToInt(clusters[i]);
            if (!argbToPopulation.ContainsKey(possibleNewCluster))
            {
                argbToPopulation[possibleNewCluster] = count;
            }
        }


        return Score(argbToPopulation).First();
    }

    public class QuantizerWu
    {
        public uint[] weights;
        public uint[] momentsR;
        public uint[] momentsG;
        public uint[] momentsB;
        public uint[] moments;
        public Box[] cubes;

        public uint Volume(Box cube, uint[] moment)
            => moment[GetIndex(cube.r1, cube.g1, cube.b1)] - moment[GetIndex(cube.r1, cube.g1, cube.b0)] - moment[GetIndex(cube.r1, cube.g0, cube.b1)] + moment[GetIndex(cube.r1, cube.g0, cube.b0)] - moment[GetIndex(cube.r0, cube.g1, cube.b1)] + moment[GetIndex(cube.r0, cube.g1, cube.b0)] + moment[GetIndex(cube.r0, cube.g0, cube.b1)] - moment[GetIndex(cube.r0, cube.g0, cube.b0)];

        public struct MaximizeResult
        {
            public int CutLocation { get; }
            public double Maximum { get; }

            public MaximizeResult(int cutLocation, double maximum)
            {
                CutLocation = cutLocation;
                Maximum = maximum;
            }
        }

        public uint Bottom(Box cube, string direction, uint[] moment)
        {
            switch (direction)
            {
                case "red":
                    return (uint)(-moment[GetIndex(cube.r0, cube.g1, cube.b1)] + moment[GetIndex(cube.r0, cube.g1, cube.b0)] + moment[GetIndex(cube.r0, cube.g0, cube.b1)] - moment[GetIndex(cube.r0, cube.g0, cube.b0)]);
                case "green":
                    return (uint)(-moment[GetIndex(cube.r1, cube.g0, cube.b1)] + moment[GetIndex(cube.r1, cube.g0, cube.b0)] + moment[GetIndex(cube.r0, cube.g0, cube.b1)] - moment[GetIndex(cube.r0, cube.g0, cube.b0)]);
                case "blue":
                    return (uint)(-moment[GetIndex(cube.r1, cube.g1, cube.b0)] + moment[GetIndex(cube.r1, cube.g0, cube.b0)] + moment[GetIndex(cube.r0, cube.g1, cube.b0)] - moment[GetIndex(cube.r0, cube.g0, cube.b0)]);
                default:
                    throw new ArgumentException("direction must be red, green, or blue");
            }
        }

        public uint Top(Box cube, string direction, uint position, uint[] moment)
        {
            switch (direction)
            {
                case "red":
                    return moment[GetIndex(position, cube.g1, cube.b1)] - moment[GetIndex(position, cube.g1, cube.b0)] - moment[GetIndex(position, cube.g0, cube.b1)] + moment[GetIndex(position, cube.g0, cube.b0)];
                case "green":
                    return moment[GetIndex(cube.r1, position, cube.b1)] - moment[GetIndex(cube.r1, position, cube.b0)] - moment[GetIndex(cube.r0, position, cube.b1)] + moment[GetIndex(cube.r0, position, cube.b0)];
                case "blue":
                    return moment[GetIndex(cube.r1, cube.g1, position)] - moment[GetIndex(cube.r1, cube.g0, position)] - moment[GetIndex(cube.r0, cube.g1, position)] + moment[GetIndex(cube.r0, cube.g0, position)];
                default:
                    throw new ArgumentException("direction must be red, green, or blue");
            }
        }

        public MaximizeResult Maximize(Box cube, string direction, uint first, uint last, uint wholeR, uint wholeG, uint wholeB, uint wholeW)
        {
            uint bottomR = Bottom(cube, direction, momentsR);
            uint bottomG = Bottom(cube, direction, momentsG);
            uint bottomB = Bottom(cube, direction, momentsB);
            uint bottomW = Bottom(cube, direction, weights);
            double max = 0;
            int cut = -1;
            uint halfR;
            uint halfG;
            uint halfB;
            uint halfW;
            for (uint i = first; i < last; i++)
            {
                halfR = bottomR + Top(cube, direction, i, momentsR);
                halfG = bottomG + Top(cube, direction, i, momentsG);
                halfB = bottomB + Top(cube, direction, i, momentsB);
                halfW = bottomW + Top(cube, direction, i, weights);
                if (halfW == 0)
                    continue;
                ulong tempNumerator = (ulong)halfR * halfR + (ulong)halfG * halfG + (ulong)halfB * halfB;
                uint tempDenominator = 1 * halfW;
                double temp = tempNumerator / (double)tempDenominator;
                halfR = wholeR - halfR;
                halfG = wholeG - halfG;
                halfB = wholeB - halfB;
                halfW = wholeW - halfW;
                if (halfW != 0)
                {
                    tempNumerator = (ulong)halfR * halfR + (ulong)halfG * halfG + (ulong)halfB * halfB;
                    tempDenominator = 1 * halfW;
                    temp += tempNumerator / (double)tempDenominator;
                    if (temp > max)
                    {
                        max = temp;
                        cut = (int)i;
                    }
                }
            }
            return new MaximizeResult(cut, max);
        }

        public bool Cut(Box one, Box two)
        {
            uint wholeR = Volume(one, momentsR);
            uint wholeG = Volume(one, momentsG);
            uint wholeB = Volume(one, momentsB);
            uint wholeW = Volume(one, weights);
            MaximizeResult maxRResult = Maximize(one, "red", one.r0 + 1, one.r1, wholeR, wholeG, wholeB, wholeW);
            MaximizeResult maxGResult = Maximize(one, "green", one.g0 + 1, one.g1, wholeR, wholeG, wholeB, wholeW);
            MaximizeResult maxBResult = Maximize(one, "blue", one.b0 + 1, one.b1, wholeR, wholeG, wholeB, wholeW);
            string direction;
            double maxR = maxRResult.Maximum;
            double maxG = maxGResult.Maximum;
            double maxB = maxBResult.Maximum;
            if (maxR >= maxG && maxR >= maxB)
            {
                if (0 > maxRResult.CutLocation)
                    return false;
                direction = "red";
            }
            else direction = maxG >= maxB && maxG >= maxB ? "green" : "blue";
            two.r1 = one.r1;
            two.g1 = one.g1;
            two.b1 = one.b1;
            switch (direction)
            {
                case "red":
                    one.r1 = (uint)maxRResult.CutLocation;
                    two.r0 = one.r1;
                    two.g0 = one.g0;
                    two.b0 = one.b0;
                    break;
                case "green":
                    one.g1 = (uint)maxGResult.CutLocation;
                    two.r0 = one.r0;
                    two.g0 = one.g1;
                    two.b0 = one.b0;
                    break;
                case "blue":
                    one.b1 = (uint)maxBResult.CutLocation;
                    two.r0 = one.r0;
                    two.g0 = one.g0;
                    two.b0 = one.b1;
                    break;
                default:
                    throw new ArgumentException("direction must be red, green, or blue");
            }
            one.vol = (one.r1 - one.r0) * (one.g1 - one.g0) * (one.b1 - one.b0);
            two.vol = (two.r1 - two.r0) * (two.g1 - two.g0) * (two.b1 - two.b0);
            return true;
        }

        public double Variance(Box cube)
        {
            uint dr = Volume(cube, momentsR);
            uint dg = Volume(cube, momentsG);
            uint db = Volume(cube, momentsB);
            uint xx = moments[GetIndex(cube.r1, cube.g1, cube.b1)]
                - moments[GetIndex(cube.r1, cube.g1, cube.b0)]
                - moments[GetIndex(cube.r1, cube.g0, cube.b1)]
                + moments[GetIndex(cube.r1, cube.g0, cube.b0)]
                - moments[GetIndex(cube.r0, cube.g1, cube.b1)]
                + moments[GetIndex(cube.r0, cube.g1, cube.b0)]
                + moments[GetIndex(cube.r0, cube.g0, cube.b1)]
                - moments[GetIndex(cube.r0, cube.g0, cube.b0)];
            ulong hypotenuse = (ulong)dr * dr + (ulong)dg * dg + (ulong)db * db;
            uint volume = Volume(cube, weights);
            return xx - hypotenuse / (double)volume;
        }

        public CreateBoxesResult CreateBoxes()
        {
            cubes = Enumerable.Range(0, 256).Select((_) => new Box()).ToArray();
            double[] volumeVariance = new double[256];
            cubes[0].r0 = 0;
            cubes[0].g0 = 0;
            cubes[0].b0 = 0;
            cubes[0].r1 = 32;
            cubes[0].g1 = 32;
            cubes[0].b1 = 32;
            uint generatedColorCount = 256;
            uint next = 0;
            for (uint i = 1; i < 256; i++)
            {
                if (Cut(cubes[next], cubes[i]))
                {
                    volumeVariance[next] = 1 < cubes[next].vol
                        ? Variance(cubes[next])
                        : 0;
                    volumeVariance[i] = 1 < cubes[i].vol
                        ? Variance(cubes[i])
                        : 0;
                }
                else
                {
                    volumeVariance[next] = 0;
                    i--;
                }
                next = 0;
                double temp = volumeVariance[0];
                for (uint j = 1; j < i; j++)
                {
                    if (volumeVariance[j] > temp)
                    {
                        temp = volumeVariance[j];
                        next = j;
                    }
                }
                if (0 >= temp)
                {
                    generatedColorCount = i + 1;
                    break;
                }
            }
            return new CreateBoxesResult(generatedColorCount);
        }
    }

    public class CreateBoxesResult
    {
        public uint ResultCount { get; }
        public CreateBoxesResult(uint resultCount)
        {
            ResultCount = resultCount;
        }
    }

    public class Box
    {
        public uint vol = 0;
        public uint b1 = 0;
        public uint b0 = 0;
        public uint g1 = 0;
        public uint g0 = 0;
        public uint r1 = 0;
        public uint r0 = 0;
    }

    public static uint GetIndex(uint r, uint g, uint b)
        => (r << 10) + (r << 6) + r + (g << 5) + g + b;
}
