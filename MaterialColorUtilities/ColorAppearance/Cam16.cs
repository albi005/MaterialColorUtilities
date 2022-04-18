using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.ColorAppearance;

public class Cam16
{
    // Transforms XYZ color space coordinates to 'cone'/'RGB' responses in CAM16.
    public static double[][] XYZ_TO_CAM16RGB =
    {
        new[] { 0.401288, 0.650173, -0.051461 },
        new[] { -0.250268, 1.204414, 0.045854 },
        new[] { -0.002079, 0.048952, 0.953127 }
    };

    // Transforms 'cone'/'RGB' responses in CAM16 to XYZ color space coordinates.
    public static double[][] CAM16RGB_TO_XYZ =
    {
        new[] { 1.8620678, -1.0112547, 0.14918678 },
        new[] { 0.38752654, 0.62144744, -0.00897398 },
        new[] { -0.01584150, -0.03412294, 1.0499644 }
    };

    public double Hue { get; set; }
    public double Chroma { get; set; }
    public double J { get; set; }
    public double Q { get; set; }
    public double M { get; set; }
    public double S { get; set; }
    public double JStar { get; set; }
    public double AStar { get; set; }
    public double BStar { get; set; }

    public double Distance(Cam16 other)
    {
        double dJ = JStar - other.JStar;
        double dA = AStar - other.AStar;
        double dB = BStar - other.BStar;
        double dEPrime = Math.Sqrt(dJ * dJ + dA * dA + dB * dB);
        double dE = 1.41 * Math.Pow(dEPrime, .63);
        return dE;
    }

    public Cam16(double hue, double chroma, double j, double q, double m, double s, double jStar, double aStar, double bStar)
    {
        Hue = hue;
        Chroma = chroma;
        J = j;
        Q = q;
        M = m;
        S = s;
        JStar = jStar;
        AStar = aStar;
        BStar = bStar;
    }

    public static Cam16 FromInt(int argb) => FromIntInViewingConditions(argb, ViewingConditions.Default);

    public static Cam16 FromIntInViewingConditions(int argb, ViewingConditions viewingConditions)
    {
        // Transform ARGB int to XYZ
        int red = (argb & 0x00ff0000) >> 16;
        int green = (argb & 0x0000ff00) >> 8;
        int blue = (argb & 0x000000ff);
        double redL = ColorUtils.Linearized(red);
        double greenL = ColorUtils.Linearized(green);
        double blueL = ColorUtils.Linearized(blue);
        double x = 0.41233895 * redL + 0.35762064 * greenL + 0.18051042 * blueL;
        double y = 0.2126 * redL + 0.7152 * greenL + 0.0722 * blueL;
        double z = 0.01932141 * redL + 0.11916382 * greenL + 0.95034478 * blueL;

        // Transform XYZ to 'cone'/'rgb' responses
        double[][] matrix = XYZ_TO_CAM16RGB;
        double rT = (x * matrix[0][0]) + (y * matrix[0][1]) + (z * matrix[0][2]);
        double gT = (x * matrix[1][0]) + (y * matrix[1][1]) + (z * matrix[1][2]);
        double bT = (x * matrix[2][0]) + (y * matrix[2][1]) + (z * matrix[2][2]);

        // Discount illuminant
        double rD = viewingConditions.RgbD[0] * rT;
        double gD = viewingConditions.RgbD[1] * gT;
        double bD = viewingConditions.RgbD[2] * bT;

        // Chromatic adaptation
        double rAF = Math.Pow(viewingConditions.Fl * Math.Abs(rD) / 100.0, 0.42);
        double gAF = Math.Pow(viewingConditions.Fl * Math.Abs(gD) / 100.0, 0.42);
        double bAF = Math.Pow(viewingConditions.Fl * Math.Abs(bD) / 100.0, 0.42);
        double rA = Math.Sign(rD) * 400.0 * rAF / (rAF + 27.13);
        double gA = Math.Sign(gD) * 400.0 * gAF / (gAF + 27.13);
        double bA = Math.Sign(bD) * 400.0 * bAF / (bAF + 27.13);

        // redness-greenness
        double a = (11.0 * rA + -12.0 * gA + bA) / 11.0;
        // yellowness-blueness
        double b = (rA + gA - 2.0 * bA) / 9.0;

        // auxiliary components
        double u = (20.0 * rA + 20.0 * gA + 21.0 * bA) / 20.0;
        double p2 = (40.0 * rA + 20.0 * gA + bA) / 20.0;

        // hue
        double atan2 = Math.Atan2(b, a);
        double atanDegrees = atan2 * 180.0 / Math.PI;
        double hue =
            atanDegrees < 0
                ? atanDegrees + 360.0
                : atanDegrees >= 360 ? atanDegrees - 360.0 : atanDegrees;
        double hueRadians = hue * Math.PI / 180.0;

        // achromatic response to color
        double ac = p2 * viewingConditions.Nbb;

        // CAM16 lightness and brightness
        double j = 100.0 * Math.Pow(
            ac / viewingConditions.Aw,
            viewingConditions.C * viewingConditions.Z);
        double q = 4.0
            / viewingConditions.C
            * Math.Sqrt(j / 100.0)
            * (viewingConditions.Aw + 4.0)
            * viewingConditions.FlRoot;

        // CAM16 chroma, colorfulness, and saturation.
        double huePrime = (hue < 20.14) ? hue + 360 : hue;
        double eHue = 0.25 * (Math.Cos(MathUtils.ToRadians(huePrime) + 2.0) + 3.8);
        double p1 = 50000.0 / 13.0 * eHue * viewingConditions.Nc * viewingConditions.Ncb;
        double t = p1 * MathUtils.Hypot(a, b) / (u + 0.305);
        double alpha = Math.Pow(1.64 - Math.Pow(0.29, viewingConditions.N), 0.73)
            * Math.Pow(t, 0.9);
        
        // CAM16 chroma, colorfulness, saturation
        double c = alpha * Math.Sqrt(j / 100.0);
        double m = c * viewingConditions.FlRoot;
        double s =
            50.0 * Math.Sqrt(alpha * viewingConditions.C / (viewingConditions.Aw + 4.0));

        // CAM16-UCS components
        double jstar = (1.0 + 100.0 * 0.007) * j / (1.0 + 0.007 * j);
        double mstar = 1.0 / 0.0228 * MathUtils.Log1p(0.0228 * m);
        double astar = mstar * Math.Cos(hueRadians);
        double bstar = mstar * Math.Sin(hueRadians);

        return new Cam16(hue, c, j, q, m, s, jstar, astar, bstar);
    }

    public static Cam16 FromJch(double j, double c, double h) => FromJchInViewingConditions(j, c, h, ViewingConditions.Default);

    private static Cam16 FromJchInViewingConditions(
        double j, double c, double h, ViewingConditions viewingConditions)
    {
        double q =
            4.0
                / viewingConditions.C
                * Math.Sqrt(j / 100.0)
                * (viewingConditions.Aw + 4.0)
                * viewingConditions.FlRoot;
        double m = c * viewingConditions.FlRoot;
        double alpha = c / Math.Sqrt(j / 100.0);
        double s =
            50.0 * Math.Sqrt(alpha * viewingConditions.C / (viewingConditions.Aw + 4.0));

        double hueRadians = h * Math.PI / 180.0;
        double jstar = (1.0 + 100.0 * 0.007) * j / (1.0 + 0.007 * j);
        double mstar = 1.0 / 0.0228 * MathUtils.Log1p(0.0228 * m);
        double astar = mstar * Math.Cos(hueRadians);
        double bstar = mstar * Math.Sin(hueRadians);
        return new Cam16(h, c, j, q, m, s, jstar, astar, bstar);
    }

    public static Cam16 FromUcs(double jstar, double astar, double bstar) =>
        FromUcsInViewingConditions(jstar, astar, bstar, ViewingConditions.Default);

    public static Cam16 FromUcsInViewingConditions(
        double jstar, double astar, double bstar, ViewingConditions viewingConditions)
    {

        double m = MathUtils.Hypot(astar, bstar);
        double m2 = MathUtils.Expm1(m * 0.0228) / 0.0228;
        double c = m2 / viewingConditions.FlRoot;
        double h = Math.Atan2(bstar, astar) * (180.0 / Math.PI);
        if (h < 0.0)
        {
            h += 360.0;
        }
        double j = jstar / (1 - (jstar - 100) * 0.007);
        return FromJchInViewingConditions(j, c, h, viewingConditions);
    }

    public int GetInt()
    {
        return Viewed(ViewingConditions.Default);
    }

    public int Viewed(ViewingConditions viewingConditions)
    {
        double alpha =
            (Chroma == 0.0 || J == 0.0)
                ? 0.0
                : Chroma / Math.Sqrt(J / 100.0);

        double t =
            
                Math.Pow(
                    alpha / Math.Pow(1.64 - Math.Pow(0.29, viewingConditions.N), 0.73), 1.0 / 0.9);
        double hRad = Hue * Math.PI / 180.0;

        double eHue = 0.25 * (Math.Cos(hRad + 2.0) + 3.8);
        double ac =
            viewingConditions.Aw
                * 
                    Math.Pow(J / 100.0, 1.0 / viewingConditions.C / viewingConditions.Z);
        double p1 = eHue * (50000.0 / 13.0) * viewingConditions.Nc * viewingConditions.Ncb;
        double p2 = ac / viewingConditions.Nbb;

        double hSin = Math.Sin(hRad);
        double hCos = Math.Cos(hRad);

        double gamma = 23.0 * (p2 + 0.305) * t / (23.0 * p1 + 11.0 * t * hCos + 108.0 * t * hSin);
        double a = gamma * hCos;
        double b = gamma * hSin;
        double rA = (460.0 * p2 + 451.0 * a + 288.0 * b) / 1403.0;
        double gA = (460.0 * p2 - 891.0 * a - 261.0 * b) / 1403.0;
        double bA = (460.0 * p2 - 220.0 * a - 6300.0 * b) / 1403.0;

        double rCBase = Math.Max(0, (27.13 * Math.Abs(rA)) / (400.0 - Math.Abs(rA)));
        double rC =
            Math.Sign(rA)
                * (100.0 / viewingConditions.Fl)
                * Math.Pow(rCBase, 1.0 / 0.42);
        double gCBase = Math.Max(0, (27.13 * Math.Abs(gA)) / (400.0 - Math.Abs(gA)));
        double gC =
            Math.Sign(gA)
                * (100.0 / viewingConditions.Fl)
                * Math.Pow(gCBase, 1.0 / 0.42);
        double bCBase = Math.Max(0, (27.13 * Math.Abs(bA)) / (400.0 - Math.Abs(bA)));
        double bC =
            Math.Sign(bA)
                * (100.0 / viewingConditions.Fl)
                * Math.Pow(bCBase, 1.0 / 0.42);
        double rF = rC / viewingConditions.RgbD[0];
        double gF = gC / viewingConditions.RgbD[1];
        double bF = bC / viewingConditions.RgbD[2];

        double[][] matrix = CAM16RGB_TO_XYZ;
        double x = (rF * matrix[0][0]) + (gF * matrix[0][1]) + (bF * matrix[0][2]);
        double y = (rF * matrix[1][0]) + (gF * matrix[1][1]) + (bF * matrix[1][2]);
        double z = (rF * matrix[2][0]) + (gF * matrix[2][1]) + (bF * matrix[2][2]);

        return ColorUtils.ArgbFromXyz(x, y, z);
    }
}
