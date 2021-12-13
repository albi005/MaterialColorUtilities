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
}
