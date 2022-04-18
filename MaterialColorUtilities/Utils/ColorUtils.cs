namespace MaterialColorUtilities.Utils;

public static class ColorUtils
{
    static readonly double[][] SrgbToXyz =
    {
        new double[] { 0.41233895, 0.35762064, 0.18051042 },
        new double[] { 0.2126, 0.7152, 0.0722 },
        new double[] { 0.01932141, 0.11916382, 0.95034478 }
    };

    static readonly double[][] XyzToSrgb =
    {
        new double[] { 3.2413774792388685, -1.5376652402851851, -0.49885366846268053 },
        new double[] { -0.9691452513005321, 1.8758853451067872, 0.04156585616912061 },
        new double[] { 0.05562093689691305, -0.20395524564742123, 1.0571799111220335 }
    };

    public static double[] WhitePointD65 { get; } = { 95.047, 100, 108.883 };

    /** Converts a color from RGB components to ARGB format. */
    public static int ArgbFromRgb(int red, int green, int blue)
    {
        return (255 << 24) | ((red & 255) << 16) | ((green & 255) << 8) | (blue & 255);
    }

    /** Returns the alpha component of a color in ARGB format. */
    public static int AlphaFromArgb(int argb)
    {
        return (argb >> 24) & 255;
    }

    /** Returns the red component of a color in ARGB format. */
    public static int RedFromArgb(int argb)
    {
        return (argb >> 16) & 255;
    }

    /** Returns the green component of a color in ARGB format. */
    public static int GreenFromArgb(int argb)
    {
        return (argb >> 8) & 255;
    }

    /** Returns the blue component of a color in ARGB format. */
    public static int BlueFromArgb(int argb)
    {
        return argb & 255;
    }

    /** Returns whether a color in ARGB format is opaque. */
    public static bool IsOpaque(int argb)
    {
        return AlphaFromArgb(argb) >= 255;
    }

    public static int ArgbFromXyz(double x, double y, double z)
    {
        double[][] matrix = XyzToSrgb;
        double linearR = matrix[0][0] * x + matrix[0][1] * y + matrix[0][2] * z;
        double linearG = matrix[1][0] * x + matrix[1][1] * y + matrix[1][2] * z;
        double linearB = matrix[2][0] * x + matrix[2][1] * y + matrix[2][2] * z;
        int r = Delinearized(linearR);
        int g = Delinearized(linearG);
        int b = Delinearized(linearB);
        return ArgbFromRgb(r, g, b);
    }

    /** Converts a color from XYZ to ARGB. */
    public static double[] XyzFromArgb(int argb)
    {
        double r = Linearized(RedFromArgb(argb));
        double g = Linearized(GreenFromArgb(argb));
        double b = Linearized(BlueFromArgb(argb));
        return MathUtils.MatrixMultiply(new double[] { r, g, b }, SrgbToXyz);
    }

    public static int ArgbFromLstar(double lstar)
    {
        double fy = (lstar + 16.0) / 116.0;
        double fz = fy;
        double fx = fy;
        double kappa = 24389.0 / 27.0;
        double epsilon = 216.0 / 24389.0;
        bool lExceedsEpsilonKappa = lstar > 8.0;
        double y = lExceedsEpsilonKappa ? fy * fy * fy : lstar / kappa;
        bool cubeExceedEpsilon = fy * fy * fy > epsilon;
        double x = cubeExceedEpsilon ? fx * fx * fx : lstar / kappa;
        double z = cubeExceedEpsilon ? fz * fz * fz : lstar / kappa;
        double[] whitePoint = WhitePointD65;
        return ArgbFromXyz(x * whitePoint[0], y * whitePoint[1], z * whitePoint[2]);
    }

    public static double LStarFromArgb(int argb)
    {
        double y = XyzFromArgb(argb)[1] / 100.0;
        double e = 216.0 / 24389.0;
        if (y <= e)
        {
            return 24389.0 / 27.0 * y;
        }
        else
        {
            double yIntermediate = Math.Pow(y, 1.0 / 3.0);
            return 116.0 * yIntermediate - 16.0;
        }
    }

    public static uint IntFromRgb(byte[] rgb) =>
        0xFF000000 |
        ((uint)rgb[0] & 255) << 16 |
        ((uint)rgb[1] & 255) << 8 |
        (uint)rgb[2] & 255;

    public static int IntFromLStar(double lStar)
    {
        double fy = (double)(lStar + 16) / 116;
        double kappa = (double)24389 / 27;
        bool cubeExceedEpsilon = fy * fy * fy > 216D / 24389;
        var xyz = new double[]
        {
            (cubeExceedEpsilon ? fy * fy * fy : (116 * fy - 16) / kappa) * WhitePointD65[0],
            (8 < lStar ? fy * fy * fy : lStar / kappa) * WhitePointD65[1],
            (cubeExceedEpsilon ? fy * fy * fy : (116 * fy - 16) / kappa) * WhitePointD65[2]
        };
        return ArgbFromXyz(xyz[0], xyz[1], xyz[2]);
    }

    public static double SanitizeDegrees(double degrees)
        => 0 > degrees ? degrees % 360 + 360 : 360 <= degrees ? degrees % 360 : degrees;

    public static int ArgbFromLab(double l, double a, double b)
    {
        double[] whitePoint = WhitePointD65;
        double fy = (l + 16.0) / 116.0;
        double fx = a / 500.0 + fy;
        double fz = fy - b / 200.0;
        double xNormalized = LabInvf(fx);
        double yNormalized = LabInvf(fy);
        double zNormalized = LabInvf(fz);
        double x = xNormalized * whitePoint[0];
        double y = yNormalized * whitePoint[1];
        double z = zNormalized * whitePoint[2];
        return ArgbFromXyz(x, y, z);
    }

    public static double[] LabFromArgb(int argb)
    {
        double linearR = Linearized(RedFromArgb(argb));
        double linearG = Linearized(GreenFromArgb(argb));
        double linearB = Linearized(BlueFromArgb(argb));
        double[][] matrix = SrgbToXyz;
        double x = matrix[0][0] * linearR + matrix[0][1] * linearG + matrix[0][2] * linearB;
        double y = matrix[1][0] * linearR + matrix[1][1] * linearG + matrix[1][2] * linearB;
        double z = matrix[2][0] * linearR + matrix[2][1] * linearG + matrix[2][2] * linearB;
        double[] whitePoint = WhitePointD65;
        double xNormalized = x / whitePoint[0];
        double yNormalized = y / whitePoint[1];
        double zNormalized = z / whitePoint[2];
        double fx = LabF(xNormalized);
        double fy = LabF(yNormalized);
        double fz = LabF(zNormalized);
        double l = 116.0 * fy - 16;
        double a = 500.0 * (fx - fy);
        double b = 200.0 * (fy - fz);
        return new double[] { l, a, b };
    }

    public static double YFromLstar(double lstar)
    {
        double ke = 8.0;
        if (lstar > ke)
        {
            return Math.Pow((lstar + 16.0) / 116.0, 3.0) * 100.0;
        }
        else
        {
            return lstar / (24389.0 / 27.0) * 100.0;
        }
    }

    public static double Linearized(int rgbComponent)
    {
        double normalized = rgbComponent / 255.0;
        if (normalized <= 0.040449936)
        {
            return normalized / 12.92 * 100.0;
        }
        else
        {
            return Math.Pow((normalized + 0.055) / 1.055, 2.4) * 100.0;
        }
    }

    public static int Delinearized(double rgbComponent)
    {
        double normalized = rgbComponent / 100.0;
        double delinearized;
        if (normalized <= 0.0031308)
        {
            delinearized = normalized * 12.92;
        }
        else
        {
            delinearized = 1.055 * Math.Pow(normalized, 1.0 / 2.4) - 0.055;
        }
        return MathUtils.ClampInt(0, 255, (int)Math.Round(delinearized * 255.0));
    }

    public static double LabF(double t)
    {
        double e = 216.0 / 24389.0;
        double kappa = 24389.0 / 27.0;
        if (t > e)
        {
            return Math.Pow(t, 1.0 / 3.0);
        }
        else
        {
            return (kappa * t + 16) / 116;
        }
    }

    public static double LabInvf(double ft)
    {
        double e = 216.0 / 24389.0;
        double kappa = 24389.0 / 27.0;
        double ft3 = ft * ft * ft;
        if (ft3 > e)
        {
            return ft3;
        }
        else
        {
            return (116 * ft - 16) / kappa;
        }
    }
}
