using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.ColorAppearance;

public class Cam16
{
    // Transforms XYZ color space coordinates to 'cone'/'RGB' responses in CAM16.
    public static float[][] XYZ_TO_CAM16RGB = new[]
    {
        new[] {0.401288f, 0.650173f, -0.051461f},
        new[] {-0.250268f, 1.204414f, 0.045854f},
        new[] {-0.002079f, 0.048952f, 0.953127f}
    };

    // Transforms 'cone'/'RGB' responses in CAM16 to XYZ color space coordinates.
    public static float[][] CAM16RGB_TO_XYZ = new[]
    {
        new[] {1.8620678f, -1.0112547f, 0.14918678f},
        new[] {0.38752654f, 0.62144744f, -0.00897398f},
        new[] {-0.01584150f, -0.03412294f, 1.0499644f}
    };

    public float Hue { get; set; }
    public float Chroma { get; set; }
    public float J { get; set; }
    public float Q { get; set; }
    public float M { get; set; }
    public float S { get; set; }
    public float JStar { get; set; }
    public float AStar { get; set; }
    public float BStar { get; set; }

    public float Distance(Cam16 other)
    {
        float dJ = JStar - other.JStar;
        float dA = AStar - other.AStar;
        float dB = BStar - other.BStar;
        double dEPrime = Math.Sqrt(dJ * dJ + dA * dA + dB * dB);
        double dE = 1.41 * Math.Pow(dEPrime, .63);
        return (float)dE;
    }

    public Cam16(float hue, float chroma, float j, float q, float m, float s, float jStar, float aStar, float bStar)
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
        float redL = (float)ColorUtils.Linearized(red);
        float greenL = (float)ColorUtils.Linearized(green);
        float blueL = (float)ColorUtils.Linearized(blue);
        float x = 0.41233895f * redL + 0.35762064f * greenL + 0.18051042f * blueL;
        float y = 0.2126f * redL + 0.7152f * greenL + 0.0722f * blueL;
        float z = 0.01932141f * redL + 0.11916382f * greenL + 0.95034478f * blueL;

        // Transform XYZ to 'cone'/'rgb' responses
        float[][] matrix = XYZ_TO_CAM16RGB;
        float rT = (x * matrix[0][0]) + (y * matrix[0][1]) + (z * matrix[0][2]);
        float gT = (x * matrix[1][0]) + (y * matrix[1][1]) + (z * matrix[1][2]);
        float bT = (x * matrix[2][0]) + (y * matrix[2][1]) + (z * matrix[2][2]);

        // Discount illuminant
        float rD = viewingConditions.RgbD[0] * rT;
        float gD = viewingConditions.RgbD[1] * gT;
        float bD = viewingConditions.RgbD[2] * bT;

        // Chromatic adaptation
        float rAF = (float)Math.Pow(viewingConditions.Fl * Math.Abs(rD) / 100.0, 0.42);
        float gAF = (float)Math.Pow(viewingConditions.Fl * Math.Abs(gD) / 100.0, 0.42);
        float bAF = (float)Math.Pow(viewingConditions.Fl * Math.Abs(bD) / 100.0, 0.42);
        float rA = Math.Sign(rD) * 400.0f * rAF / (rAF + 27.13f);
        float gA = Math.Sign(gD) * 400.0f * gAF / (gAF + 27.13f);
        float bA = Math.Sign(bD) * 400.0f * bAF / (bAF + 27.13f);

        // redness-greenness
        float a = (float)(11.0 * rA + -12.0 * gA + bA) / 11.0f;
        // yellowness-blueness
        float b = (float)(rA + gA - 2.0 * bA) / 9.0f;

        // auxiliary components
        float u = (20.0f * rA + 20.0f * gA + 21.0f * bA) / 20.0f;
        float p2 = (40.0f * rA + 20.0f * gA + bA) / 20.0f;

        // hue
        float atan2 = (float)Math.Atan2(b, a);
        float atanDegrees = atan2 * 180.0f / (float)Math.PI;
        float hue =
            atanDegrees < 0
                ? atanDegrees + 360.0f
                : atanDegrees >= 360 ? atanDegrees - 360.0f : atanDegrees;
        float hueRadians = hue * (float)Math.PI / 180.0f;

        // achromatic response to color
        float ac = p2 * viewingConditions.Nbb;

        // CAM16 lightness and brightness
        float j =
            100.0f
                * (float)
                    Math.Pow(
                        ac / viewingConditions.Aw,
                        viewingConditions.C * viewingConditions.Z);
        float q =
            4.0f
                / viewingConditions.C
                * (float)Math.Sqrt(j / 100.0f)
                * (viewingConditions.Aw + 4.0f)
                * viewingConditions.FlRoot;

        // CAM16 chroma, colorfulness, and saturation.
        float huePrime = (hue < 20.14) ? hue + 360 : hue;
        float eHue = 0.25f * (float)(Math.Cos(MathUtils.ToRadians(huePrime) + 2.0) + 3.8);
        float p1 = 50000.0f / 13.0f * eHue * viewingConditions.Nc * viewingConditions.Ncb;
        float t = p1 * (float)MathUtils.Hypot(a, b) / (u + 0.305f);
        float alpha =
            (float)Math.Pow(1.64 - Math.Pow(0.29, viewingConditions.N), 0.73)
                * (float)Math.Pow(t, 0.9);
        // CAM16 chroma, colorfulness, saturation
        float c = alpha * (float)Math.Sqrt(j / 100.0);
        float m = c * viewingConditions.FlRoot;
        float s =
            50.0f
                * (float)
                    Math.Sqrt(alpha * viewingConditions.C / (viewingConditions.Aw + 4.0f));

        // CAM16-UCS components
        float jstar = (1.0f + 100.0f * 0.007f) * j / (1.0f + 0.007f * j);
        float mstar = 1.0f / 0.0228f * (float)MathUtils.Log1p(0.0228f * m);
        float astar = mstar * (float)Math.Cos(hueRadians);
        float bstar = mstar * (float)Math.Sin(hueRadians);

        return new Cam16(hue, c, j, q, m, s, jstar, astar, bstar);
    }

    public static Cam16 FromJch(float j, float c, float h) => FromJchInViewingConditions(j, c, h, ViewingConditions.Default);

    private static Cam16 FromJchInViewingConditions(
        float j, float c, float h, ViewingConditions viewingConditions)
    {
        float q =
            4.0f
                / viewingConditions.C
                * (float)Math.Sqrt(j / 100.0)
                * (viewingConditions.Aw + 4.0f)
                * viewingConditions.FlRoot;
        float m = c * viewingConditions.FlRoot;
        float alpha = c / (float)Math.Sqrt(j / 100.0);
        float s =
            50.0f
                * (float)
                    Math.Sqrt(alpha * viewingConditions.C / (viewingConditions.Aw + 4.0f));

        float hueRadians = h * (float)Math.PI / 180.0f;
        float jstar = (1.0f + 100.0f * 0.007f) * j / (1.0f + 0.007f * j);
        float mstar = 1.0f / 0.0228f * (float)MathUtils.Log1p(0.0228 * m);
        float astar = mstar * (float)Math.Cos(hueRadians);
        float bstar = mstar * (float)Math.Sin(hueRadians);
        return new Cam16(h, c, j, q, m, s, jstar, astar, bstar);
    }

    public static Cam16 FromUcs(float jstar, float astar, float bstar) =>
        FromUcsInViewingConditions(jstar, astar, bstar, ViewingConditions.Default);

    public static Cam16 FromUcsInViewingConditions(
        float jstar, float astar, float bstar, ViewingConditions viewingConditions)
    {

        double m = MathUtils.Hypot(astar, bstar);
        double m2 = MathUtils.Expm1(m * 0.0228f) / 0.0228f;
        double c = m2 / viewingConditions.FlRoot;
        double h = Math.Atan2(bstar, astar) * (180.0f / Math.PI);
        if (h < 0.0)
        {
            h += 360.0f;
        }
        float j = jstar / (1f - (jstar - 100f) * 0.007f);
        return FromJchInViewingConditions(j, (float)c, (float)h, viewingConditions);
    }

    public int GetInt()
    {
        return Viewed(ViewingConditions.Default);
    }

    public int Viewed(ViewingConditions viewingConditions)
    {
        float alpha =
            (Chroma == 0.0 || J == 0.0)
                ? 0.0f
                : Chroma / (float)Math.Sqrt(J / 100.0);

        float t =
            (float)
                Math.Pow(
                    alpha / Math.Pow(1.64 - Math.Pow(0.29, viewingConditions.N), 0.73), 1.0 / 0.9);
        float hRad = Hue * (float)Math.PI / 180.0f;

        float eHue = 0.25f * (float)(Math.Cos(hRad + 2.0) + 3.8);
        float ac =
            viewingConditions.Aw
                * (float)
                    Math.Pow(J / 100.0, 1.0 / viewingConditions.C / viewingConditions.Z);
        float p1 = eHue * (50000.0f / 13.0f) * viewingConditions.Nc * viewingConditions.Ncb;
        float p2 = ac / viewingConditions.Nbb;

        float hSin = (float)Math.Sin(hRad);
        float hCos = (float)Math.Cos(hRad);

        float gamma = 23.0f * (p2 + 0.305f) * t / (23.0f * p1 + 11.0f * t * hCos + 108.0f * t * hSin);
        float a = gamma * hCos;
        float b = gamma * hSin;
        float rA = (460.0f * p2 + 451.0f * a + 288.0f * b) / 1403.0f;
        float gA = (460.0f * p2 - 891.0f * a - 261.0f * b) / 1403.0f;
        float bA = (460.0f * p2 - 220.0f * a - 6300.0f * b) / 1403.0f;

        float rCBase = (float)Math.Max(0, (27.13 * Math.Abs(rA)) / (400.0 - Math.Abs(rA)));
        float rC =
            Math.Sign(rA)
                * (100.0f / viewingConditions.Fl)
                * (float)Math.Pow(rCBase, 1.0 / 0.42);
        float gCBase = (float)Math.Max(0, (27.13 * Math.Abs(gA)) / (400.0 - Math.Abs(gA)));
        float gC =
            Math.Sign(gA)
                * (100.0f / viewingConditions.Fl)
                * (float)Math.Pow(gCBase, 1.0 / 0.42);
        float bCBase = (float)Math.Max(0, (27.13 * Math.Abs(bA)) / (400.0 - Math.Abs(bA)));
        float bC =
            Math.Sign(bA)
                * (100.0f / viewingConditions.Fl)
                * (float)Math.Pow(bCBase, 1.0 / 0.42);
        float rF = rC / viewingConditions.RgbD[0];
        float gF = gC / viewingConditions.RgbD[1];
        float bF = bC / viewingConditions.RgbD[2];

        float[][] matrix = CAM16RGB_TO_XYZ;
        float x = (rF * matrix[0][0]) + (gF * matrix[0][1]) + (bF * matrix[0][2]);
        float y = (rF * matrix[1][0]) + (gF * matrix[1][1]) + (bF * matrix[1][2]);
        float z = (rF * matrix[2][0]) + (gF * matrix[2][1]) + (bF * matrix[2][2]);

        return ColorUtils.ArgbFromXyz(x, y, z);
    }
}
