using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Quantize;

/**
 * An image quantizer that divides the image's pixels into clusters by recursively cutting an RGB
 * cube, based on the weight of pixels in each area of the cube.
 *
 * <p>The algorithm was described by Xiaolin Wu in Graphic Gems II, published in 1991.
 */
public class QuantizerWu : IQuantizer
{
    int[] weights;
    int[] momentsR;
    int[] momentsG;
    int[] momentsB;
    double[] moments;
    Box[] cubes;

    // A histogram of all the input colors is constructed. It has the shape of a
    // cube. The cube would be too large if it contained all 16 million colors:
    // historical best practice is to use 5 bits  of the 8 in each channel,
    // reducing the histogram to a volume of ~32,000.
    private const int INDEX_BITS = 5;
    private const int INDEX_COUNT = 33; // ((1 << INDEX_BITS) + 1)
    private const int TOTAL_SIZE = 35937; // INDEX_COUNT * INDEX_COUNT * INDEX_COUNT

    public QuantizerResult Quantize(int[] pixels, int colorCount)
    {
        QuantizerResult mapResult = new QuantizerMap().Quantize(pixels, colorCount);
        ConstructHistogram(mapResult.ColorToCount);
        CreateMoments();
        CreateBoxesResult createBoxesResult = CreateBoxes(colorCount);
        List<int> colors = CreateResult(createBoxesResult.ResultCount);
        Dictionary<int, int> resultMap = new();
        foreach (int color in colors)
        {
            resultMap[color] = 0;
        }
        return new QuantizerResult(resultMap);
    }

    static int GetIndex(int r, int g, int b)
        => (r << (INDEX_BITS * 2)) + (r << (INDEX_BITS + 1)) + r + (g << INDEX_BITS) + g + b;

    void ConstructHistogram(Dictionary<int, int> pixels)
    {
        weights = new int[TOTAL_SIZE];
        momentsR = new int[TOTAL_SIZE];
        momentsG = new int[TOTAL_SIZE];
        momentsB = new int[TOTAL_SIZE];
        moments = new double[TOTAL_SIZE];

        foreach (var pair in pixels)
        {
            int pixel = pair.Key;
            int count = pair.Value;
            int red = ColorUtils.RedFromArgb(pixel);
            int green = ColorUtils.GreenFromArgb(pixel);
            int blue = ColorUtils.BlueFromArgb(pixel);
            int bitsToRemove = 8 - INDEX_BITS;
            int iR = (red >> bitsToRemove) + 1;
            int iG = (green >> bitsToRemove) + 1;
            int iB = (blue >> bitsToRemove) + 1;
            int index = GetIndex(iR, iG, iB);
            weights[index] += count;
            momentsR[index] += (red * count);
            momentsG[index] += (green * count);
            momentsB[index] += (blue * count);
            moments[index] += (count * ((red * red) + (green * green) + (blue * blue)));
        }
    }

    void CreateMoments()
    {
        for (int r = 1; r < INDEX_COUNT; ++r)
        {
            int[] area = new int[INDEX_COUNT];
            int[] areaR = new int[INDEX_COUNT];
            int[] areaG = new int[INDEX_COUNT];
            int[] areaB = new int[INDEX_COUNT];
            double[] area2 = new double[INDEX_COUNT];

            for (int g = 1; g < INDEX_COUNT; ++g)
            {
                int line = 0;
                int lineR = 0;
                int lineG = 0;
                int lineB = 0;
                double line2 = 0.0;
                for (int b = 1; b < INDEX_COUNT; ++b)
                {
                    int index = GetIndex(r, g, b);
                    line += weights[index];
                    lineR += momentsR[index];
                    lineG += momentsG[index];
                    lineB += momentsB[index];
                    line2 += moments[index];

                    area[b] += line;
                    areaR[b] += lineR;
                    areaG[b] += lineG;
                    areaB[b] += lineB;
                    area2[b] += line2;

                    int previousIndex = GetIndex(r - 1, g, b);
                    weights[index] = weights[previousIndex] + area[b];
                    momentsR[index] = momentsR[previousIndex] + areaR[b];
                    momentsG[index] = momentsG[previousIndex] + areaG[b];
                    momentsB[index] = momentsB[previousIndex] + areaB[b];
                    moments[index] = moments[previousIndex] + area2[b];
                }
            }
        }
    }

    CreateBoxesResult CreateBoxes(int maxColorCount)
    {
        cubes = new Box[maxColorCount];
        for (int i = 0; i < maxColorCount; i++)
        {
            cubes[i] = new Box();
        }
        double[] volumeVariance = new double[maxColorCount];
        Box firstBox = cubes[0];
        firstBox.R1 = INDEX_COUNT - 1;
        firstBox.G1 = INDEX_COUNT - 1;
        firstBox.B1 = INDEX_COUNT - 1;

        int generatedColorCount = maxColorCount;
        int next = 0;
        for (int i = 1; i < maxColorCount; i++)
        {
            if (Cut(cubes[next], cubes[i]))
            {
                volumeVariance[next] = (cubes[next].Vol > 1) ? Variance(cubes[next]) : 0.0;
                volumeVariance[i] = (cubes[i].Vol > 1) ? Variance(cubes[i]) : 0.0;
            }
            else
            {
                volumeVariance[next] = 0.0;
                i--;
            }

            next = 0;

            double temp = volumeVariance[0];
            for (int j = 1; j <= i; j++)
            {
                if (volumeVariance[j] > temp)
                {
                    temp = volumeVariance[j];
                    next = j;
                }
            }
            if (temp <= 0.0)
            {
                generatedColorCount = i + 1;
                break;
            }
        }

        return new CreateBoxesResult(maxColorCount, generatedColorCount);
    }

    List<int> CreateResult(int colorCount)
    {
        List<int> colors = new();
        for (int i = 0; i < colorCount; ++i)
        {
            Box cube = cubes[i];
            int weight = Volume(cube, weights);
            if (weight > 0)
            {
                int r = Volume(cube, momentsR) / weight;
                int g = Volume(cube, momentsG) / weight;
                int b = Volume(cube, momentsB) / weight;
                int color = (255 << 24) | ((r & 0x0ff) << 16) | ((g & 0x0ff) << 8) | (b & 0x0ff);
                colors.Add(color);
            }
        }
        return colors;
    }

    double Variance(Box cube)
    {
        int dr = Volume(cube, momentsR);
        int dg = Volume(cube, momentsG);
        int db = Volume(cube, momentsB);
        double xx =
            moments[GetIndex(cube.R1, cube.G1, cube.B1)]
            - moments[GetIndex(cube.R1, cube.G1, cube.B0)]
            - moments[GetIndex(cube.R1, cube.G0, cube.B1)]
            + moments[GetIndex(cube.R1, cube.G0, cube.B0)]
            - moments[GetIndex(cube.R0, cube.G1, cube.B1)]
            + moments[GetIndex(cube.R0, cube.G1, cube.B0)]
            + moments[GetIndex(cube.R0, cube.G0, cube.B1)]
            - moments[GetIndex(cube.R0, cube.G0, cube.B0)];

        int hypotenuse = dr * dr + dg * dg + db * db;
        int volume = Volume(cube, weights);
        return xx - (hypotenuse / volume);
    }

    bool Cut(Box one, Box two)
    {
        int wholeR = Volume(one, momentsR);
        int wholeG = Volume(one, momentsG);
        int wholeB = Volume(one, momentsB);
        int wholeW = Volume(one, weights);

        MaximizeResult maxRResult =
            Maximize(one, Direction.RED, one.R0 + 1, one.R1, wholeR, wholeG, wholeB, wholeW);
        MaximizeResult maxGResult =
            Maximize(one, Direction.GREEN, one.G0 + 1, one.G1, wholeR, wholeG, wholeB, wholeW);
        MaximizeResult maxBResult =
            Maximize(one, Direction.BLUE, one.B0 + 1, one.B1, wholeR, wholeG, wholeB, wholeW);
        Direction cutDirection;
        double maxR = maxRResult.Maximum;
        double maxG = maxGResult.Maximum;
        double maxB = maxBResult.Maximum;
        if (maxR >= maxG && maxR >= maxB)
        {
            if (maxRResult.CutLocation < 0)
            {
                return false;
            }
            cutDirection = Direction.RED;
        }
        else if (maxG >= maxR && maxG >= maxB)
        {
            cutDirection = Direction.GREEN;
        }
        else
        {
            cutDirection = Direction.BLUE;
        }

        two.R1 = one.R1;
        two.G1 = one.G1;
        two.B1 = one.B1;

        switch (cutDirection)
        {
            case Direction.RED:
                one.R1 = maxRResult.CutLocation;
                two.R0 = one.R1;
                two.G0 = one.G0;
                two.B0 = one.B0;
                break;
            case Direction.GREEN:
                one.G1 = maxGResult.CutLocation;
                two.R0 = one.R0;
                two.G0 = one.G1;
                two.B0 = one.B0;
                break;
            case Direction.BLUE:
                one.B1 = maxBResult.CutLocation;
                two.R0 = one.R0;
                two.G0 = one.G0;
                two.B0 = one.B1;
                break;
        }

        one.Vol = (one.R1 - one.R0) * (one.G1 - one.G0) * (one.B1 - one.B0);
        two.Vol = (two.R1 - two.R0) * (two.G1 - two.G0) * (two.B1 - two.B0);

        return true;
    }

    MaximizeResult Maximize(
        Box cube,
        Direction direction,
        int first,
        int last,
        int wholeR,
        int wholeG,
        int wholeB,
        int wholeW)
    {
        int bottomR = Bottom(cube, direction, momentsR);
        int bottomG = Bottom(cube, direction, momentsG);
        int bottomB = Bottom(cube, direction, momentsB);
        int bottomW = Bottom(cube, direction, weights);

        double max = 0.0;
        int cut = -1;

        int halfR;
        int halfG;
        int halfB;
        int halfW;
        for (int i = first; i < last; i++)
        {
            halfR = bottomR + Top(cube, direction, i, momentsR);
            halfG = bottomG + Top(cube, direction, i, momentsG);
            halfB = bottomB + Top(cube, direction, i, momentsB);
            halfW = bottomW + Top(cube, direction, i, weights);
            if (halfW == 0)
            {
                continue;
            }

            double tempNumerator = halfR * halfR + halfG * halfG + halfB * halfB;
            double tempDenominator = halfW;
            double temp = tempNumerator / tempDenominator;

            halfR = wholeR - halfR;
            halfG = wholeG - halfG;
            halfB = wholeB - halfB;
            halfW = wholeW - halfW;
            if (halfW == 0)
            {
                continue;
            }

            tempNumerator = halfR * halfR + halfG * halfG + halfB * halfB;
            tempDenominator = halfW;
            temp += (tempNumerator / tempDenominator);

            if (temp > max)
            {
                max = temp;
                cut = i;
            }
        }
        return new MaximizeResult(cut, max);
    }

    private static int Volume(Box cube, int[] moment) => 
        moment[GetIndex(cube.R1, cube.G1, cube.B1)]
        - moment[GetIndex(cube.R1, cube.G1, cube.B0)]
        - moment[GetIndex(cube.R1, cube.G0, cube.B1)]
        + moment[GetIndex(cube.R1, cube.G0, cube.B0)]
        - moment[GetIndex(cube.R0, cube.G1, cube.B1)]
        + moment[GetIndex(cube.R0, cube.G1, cube.B0)]
        + moment[GetIndex(cube.R0, cube.G0, cube.B1)]
        - moment[GetIndex(cube.R0, cube.G0, cube.B0)];

    private static int Bottom(Box cube, Direction direction, int[] moment) => direction switch
    {
        Direction.RED =>
            -moment[GetIndex(cube.R0, cube.G1, cube.B1)]
            + moment[GetIndex(cube.R0, cube.G1, cube.B0)]
            + moment[GetIndex(cube.R0, cube.G0, cube.B1)]
            - moment[GetIndex(cube.R0, cube.G0, cube.B0)],
        Direction.GREEN =>
            -moment[GetIndex(cube.R1, cube.G0, cube.B1)]
            + moment[GetIndex(cube.R1, cube.G0, cube.B0)]
            + moment[GetIndex(cube.R0, cube.G0, cube.B1)]
            - moment[GetIndex(cube.R0, cube.G0, cube.B0)],
        Direction.BLUE =>
            -moment[GetIndex(cube.R1, cube.G1, cube.B0)]
            + moment[GetIndex(cube.R1, cube.G0, cube.B0)]
            + moment[GetIndex(cube.R0, cube.G1, cube.B0)]
            - moment[GetIndex(cube.R0, cube.G0, cube.B0)],
        _ => throw new ArgumentException("Unexpected direction: " + direction),
    };

    private static int Top(Box cube, Direction direction, int position, int[] moment) => direction switch
    {
        Direction.RED =>
            moment[GetIndex(position, cube.G1, cube.B1)]
            - moment[GetIndex(position, cube.G1, cube.B0)]
            - moment[GetIndex(position, cube.G0, cube.B1)]
            + moment[GetIndex(position, cube.G0, cube.B0)],
        Direction.GREEN =>
            moment[GetIndex(cube.R1, position, cube.B1)]
            - moment[GetIndex(cube.R1, position, cube.B0)]
            - moment[GetIndex(cube.R0, position, cube.B1)]
            + moment[GetIndex(cube.R0, position, cube.B0)],
        Direction.BLUE =>
            moment[GetIndex(cube.R1, cube.G1, position)]
            - moment[GetIndex(cube.R1, cube.G0, position)]
            - moment[GetIndex(cube.R0, cube.G1, position)]
            + moment[GetIndex(cube.R0, cube.G0, position)],
        _ => throw new ArgumentException("Unexpected direction: " + direction),
    };

    public enum Direction
    {
        RED,
        GREEN,
        BLUE
    }

    public class MaximizeResult
    {
        // < 0 if cut impossible
        public int CutLocation { get; set; }
        public double Maximum { get; set; }

        public MaximizeResult(int cut, double max)
        {
            CutLocation = cut;
            Maximum = max;
        }
    }

    public class CreateBoxesResult
    {
        public int RequestedCount { get; set; }
        public int ResultCount { get; set; }

        public CreateBoxesResult(int requestedCount, int resultCount)
        {
            RequestedCount = requestedCount;
            ResultCount = resultCount;
        }
    }

    public class Box
    {
        public int R0 { get; set; } = 0;
        public int R1 { get; set; } = 0;
        public int G0 { get; set; } = 0;
        public int G1 { get; set; } = 0;
        public int B0 { get; set; } = 0;
        public int B1 { get; set; } = 0;
        public int Vol { get; set; } = 0;
    }
}
