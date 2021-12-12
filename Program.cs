var result = HCT.GetIntInViewingConditions(52.34833488031388, 16, 99);
Console.Write(result);

public record Cam16(
    double hue,
    double chroma,
    double j,
    double q,
    double s,
    double jstar,
    double astar,
    double bstar)
{
    public double Distance(Cam16 other)
    {
        double dJ = jstar - other.jstar;
        double dA = astar - other.astar;
        double dB = bstar - other.bstar;
        return 1.41 * Math.Pow(Math.Sqrt(dJ * dJ + dA * dA + dB * dB), .63);
    }

    public static Cam16 FromJchInViewingConditions(double j, double c, double h)
    {
        double hueRadians = h * Math.PI / 180;
        double mstar = 1 / .0228 * Math.Log(1 + .0228 * c * ViewingConditions.DEFAULT.fLRoot);
        return new(
            h,
            c,
            j,
            4 / ViewingConditions.DEFAULT.c * Math.Sqrt(j / 100) * (ViewingConditions.DEFAULT.aw + 4) * ViewingConditions.DEFAULT.fLRoot,
            50 * Math.Sqrt(c / Math.Sqrt(j / 100) * ViewingConditions.DEFAULT.c / (ViewingConditions.DEFAULT.aw + 4)),
            (1 + 100 * .007) * j / (1 + .007 * j),
            mstar * Math.Cos(hueRadians),
            mstar * Math.Sin(hueRadians));
    }

    public static Cam16 FromIntInViewingConditions(uint argb)
    {
        var redL = 100 * Utils.Linearized((double)((argb & 16711680) >> 16) / 255);
        var greenL = 100 * Utils.Linearized((double)((argb & 65280) >> 8) / 255);
        var blueL = 100 * Utils.Linearized((double)(argb & 255) / 255);
        var x = .41233895 * redL + .35762064 * greenL + .18051042 * blueL;
        var y = .2126 * redL + .7152 * greenL + .0722 * blueL;
        var z = .01932141 * redL + .11916382 * greenL + .95034478 * blueL;
        var rD = ViewingConditions.DEFAULT.rgbD[0] * (.401288 * x + .650173 * y - .051461 * z);
        var gD = ViewingConditions.DEFAULT.rgbD[1] * (-.250268 * x + 1.204414 * y + .045854 * z);
        var bD = ViewingConditions.DEFAULT.rgbD[2] * (-.002079 * x + .048952 * y + .953127 * z);
        var rAF = Math.Pow(ViewingConditions.DEFAULT.fl * Math.Abs(rD) / 100, .42);
        var gAF = Math.Pow(ViewingConditions.DEFAULT.fl * Math.Abs(gD) / 100, .42);
        var bAF = Math.Pow(ViewingConditions.DEFAULT.fl * Math.Abs(bD) / 100, .42);
        var rA = 400 * Math.Sign(rD) * rAF / (rAF + 27.13);
        var gA = 400 * Math.Sign(gD) * gAF / (gAF + 27.13);
        var bA = 400 * Math.Sign(bD) * bAF / (bAF + 27.13);
        var a = (11 * rA + -12 * gA + bA) / 11;
        var b = (rA + gA - 2 * bA) / 9;
        var atanDegrees = 180 * Math.Atan2(b, a) / Math.PI;
        var hue = 0 > atanDegrees ? atanDegrees + 360 : 360 <= atanDegrees ? atanDegrees - 360 : atanDegrees;
        var hueRadians = hue * Math.PI / 180;
        var j = 100 * Math.Pow((40 * rA + 20 * gA + bA) / 20 * ViewingConditions.DEFAULT.nbb / ViewingConditions.DEFAULT.aw, ViewingConditions.DEFAULT.c * ViewingConditions.DEFAULT.z);
        var alpha = Math.Pow(5E4 / 13 * .25 * (Math.Cos((20.14 > hue ? hue + 360 : hue) * Math.PI / 180 + 2) + 3.8) * ViewingConditions.DEFAULT.nc * ViewingConditions.DEFAULT.ncb * Math.Sqrt(a * a + b * b) / ((20 * rA + 20 * gA + 21 * bA) / 20 + .305), .9) * Math.Pow(1.64 - Math.Pow(.29, ViewingConditions.DEFAULT.n), .73);
        var c = alpha * Math.Sqrt(j / 100);
        var mstar = 1 / .0228 * Math.Log(1 + .0228 * c * ViewingConditions.DEFAULT.fLRoot);
        return new(
            hue,
            c,
            j,
            4 / ViewingConditions.DEFAULT.c * Math.Sqrt(j / 100) * (ViewingConditions.DEFAULT.aw + 4) * ViewingConditions.DEFAULT.fLRoot,
            50 * Math.Sqrt(alpha * ViewingConditions.DEFAULT.c / (ViewingConditions.DEFAULT.aw + +4)),
            (1 + 100 * .007) * j / (1 + .007 * j),
            mstar * Math.Cos(hueRadians),
            mstar * Math.Sin(hueRadians));
    }

}

public record ViewingConditions(
    double n,
    double aw,
    double nbb,
    double ncb,
    double c,
    double nc,
    double[] rgbD,
    double fl,
    double fLRoot,
    double z)
{
    public static ViewingConditions DEFAULT = Default();

    public static ViewingConditions Default(
        double[]? whitePoint = null,
        double adaptingLuminance = 0,
        double backgroundLstar = 50,
        double surround = 2,
        bool discountingIlluminant = false
        )
    {
        if (whitePoint == null)
            whitePoint = ColorUtils.WHITE_POINT_D65;
        if (adaptingLuminance == 0)
            adaptingLuminance = 200 / Math.PI * 100 * Math.Pow((double)66 / 116, 3) / 100;

        double rW = .401288 * whitePoint[0] + .650173 * whitePoint[1] + -.051461 * whitePoint[2];
        double gW = -.250268 * whitePoint[0] + 1.204414 * whitePoint[1] + .045854 * whitePoint[2];
        double bW = -.002079 * whitePoint[0] + .048952 * whitePoint[1] + .953127 * whitePoint[2];
        double f = .8 + surround / 10;

        double temp;
        if (.9 <= f)
        {
            var amount = 10 * (f - .9);
            temp = .59 * (1 - amount) + .69 * amount;
        }
        else
        {
            var amountJscomp0 = 10 * (f - .8);
            temp = .525 * (1 - amountJscomp0) + .59 * amountJscomp0;
        }
        var d = discountingIlluminant ? 1 : f * (1 - 1 / 3.6 * Math.Exp((-(double)adaptingLuminance - 42) / 92));
        d = 1 < d ? 1 : 0 > d ? 0 : d;
        var rgbD = new double[] { 100 / rW * d + 1 - d, 100 / gW * d + 1 - d, 100 / bW * d + 1 - d };
        double k = 1 / (5 * adaptingLuminance + 1);
        double k4 = k * k * k * k;
        double k4F = 1 - k4;
        double fl = k4 * adaptingLuminance + .1 * k4F * k4F * Math.Cbrt(5 * adaptingLuminance);
        double n = (8 < backgroundLstar ? 100 * Math.Pow((backgroundLstar + 16) / 116, 3) : backgroundLstar / ((double)24389 / 27) * 100) / whitePoint[1];
        double nbb = .725 / Math.Pow(n, .2);
        var rgbAFactors = new double[] { Math.Pow(fl * rgbD[0] * rW / 100, .42), Math.Pow(fl * rgbD[1] * gW / 100, .42), Math.Pow(fl * rgbD[2] * bW / 100, .42) };
        var rgbA = new double[] { 400 * rgbAFactors[0] / (rgbAFactors[0] + 27.13), 400 * rgbAFactors[1] / (rgbAFactors[1] + 27.13), 400 * rgbAFactors[2] / (rgbAFactors[2] + 27.13) };
        return new(n, (2 * rgbA[0] + rgbA[1] + .05 * rgbA[2]) * nbb, nbb, nbb, temp, f, rgbD, fl, Math.Pow(fl, .25), 1.48 + Math.Sqrt(n));
    }
}

public static class ColorUtils
{
    public static double[] WHITE_POINT_D65 = new double[] { 95.047, 100, 108.883 };

}

public class HCT
{
    private readonly double internalHue;
    private readonly double internalChroma;
    private readonly double internalTone;

    public HCT(double internalHue, double internalChroma, double internalTone)
    {
        this.internalHue = internalHue;
        this.internalChroma = internalChroma;
        this.internalTone = internalTone;
        //SetInternalState(this, ToInt());
    }

    private uint ToInt()
        => GetIntInViewingConditions(Utils.SanitizeDegrees(internalHue), internalChroma, Math.Clamp(internalTone, 0, 100));

    public static uint GetIntInViewingConditions(double hue, double chroma, double tone)
    {
        if (1 > chroma || 0 >= Math.Round(tone) || 100 <= Math.Round(tone))
            return Utils.IntFromLstar(tone);

        hue = Utils.SanitizeDegrees(hue);
        double high = chroma;
        double mid = chroma;
        double low = 0;
        bool isFirstLoop = true;
        Cam16? answer = null;
        for (; .4 <= Math.Abs(low - high);)
        {
            var hueInner = hue;
            var chromaInner = mid;
            var toneInner = tone;
            double lowInner = 0; double highInner = 100; double midInner, bestdL = 1E3, bestdE = 1E3; Cam16? bestCam = null;
            for (; .01 < Math.Abs(lowInner - highInner);)
            {
                midInner = lowInner + (highInner - lowInner) / 2;
                uint clipped = Utils.Viewed(Cam16.FromJchInViewingConditions(midInner, chromaInner, hueInner));
                double clippedLstar = Utils.LstarFromInt(clipped);
                double dL = Math.Abs(toneInner - clippedLstar);
                if (.2 > dL)
                {
                    Cam16 camClipped = Cam16.FromIntInViewingConditions(clipped);
                    double dE = camClipped.Distance(Cam16.FromJchInViewingConditions(camClipped.j, camClipped.chroma, hueInner));
                    if (1 >= dE && dE <= bestdE)
                    {
                        bestdL = dL;
                        bestdE = dE;
                        bestCam = camClipped;
                    }
                }
                if (0 == bestdL && 0 == bestdE)
                    break;
                if (clippedLstar < toneInner) lowInner = midInner; else highInner = midInner;
            }
            Cam16? possibleAnswer = bestCam;
            if (isFirstLoop)
            {
                if (null != possibleAnswer)
                    return Utils.Viewed(possibleAnswer);
                isFirstLoop = false;
            }
            else
                if (null == possibleAnswer) high = mid;
            else
            {
                answer = possibleAnswer;
                low = mid;
            }
            mid = low + (high - low) / 2;
        }
        return null == answer ? Utils.IntFromLstar(tone) : Utils.Viewed(answer);
    }
}

public static class Utils
{
    public static double Linearized(double rgb) => .04045 >= rgb ? rgb / 12.92 : Math.Pow((rgb + .055) / 1.055, 2.4);
    public static double Delinearized(double rgb) => .0031308 >= rgb ? 12.92 * rgb : 1.055 * Math.Pow(rgb, 1 / 2.4) - .055;

    public static double LstarFromInt(uint argb)
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

    public static uint IntFromLstar(double lstar)
    {
        double fy = (double)(lstar + 16) / 116;
        double kappa = (double)24389 / 27;
        bool cubeExceedEpsilon = fy * fy * fy > 216D / 24389;
        var xyz = new double[]
        {
            (cubeExceedEpsilon ? fy * fy * fy : (116 * fy - 16) / kappa) * ColorUtils.WHITE_POINT_D65[0],
            (8 < lstar ? fy * fy * fy : lstar / kappa) * ColorUtils.WHITE_POINT_D65[1],
            (cubeExceedEpsilon ? fy * fy * fy : (116 * fy - 16) / kappa) * ColorUtils.WHITE_POINT_D65[2]
        };
        return IntFromXyzComponents(xyz[0], xyz[1], xyz[2]);
    }

    public static double SanitizeDegrees(double degrees)
        => 0 > degrees ? degrees % 360 + 360 : 360 <= degrees ? degrees % 360 : degrees;

    public static uint Viewed(Cam16 self)
    {
        double t = Math.Pow((0 == self.chroma || 0 == self.j ? 0 : self.chroma / Math.Sqrt(self.j / 100)) / Math.Pow(1.64 - Math.Pow(.29, ViewingConditions.DEFAULT.n), .73), 1 / .9);
        double hRad = self.hue * Math.PI / 180;
        double p2 = ViewingConditions.DEFAULT.aw * Math.Pow(self.j / 100, 1 / ViewingConditions.DEFAULT.c / ViewingConditions.DEFAULT.z) / ViewingConditions.DEFAULT.nbb;
        double hSin = Math.Sin(hRad);
        double hCos = Math.Cos(hRad);
        double gamma = 23 * (p2 + .305) * t / (5E4 / 13 * (Math.Cos(hRad + 2) + 3.8) * 5.75 * ViewingConditions.DEFAULT.nc * ViewingConditions.DEFAULT.ncb + 11 * t * hCos + 108 * t * hSin);
        double a = gamma * hCos;
        double b = gamma * hSin;
        double rA = (460 * p2 + 451 * a + 288 * b) / 1403;
        double gA = (460 * p2 - 891 * a - 261 * b) / 1403;
        double bA = (460 * p2 - 220 * a - 6300 * b) / 1403;
        double rF = 100 / ViewingConditions.DEFAULT.fl * Math.Sign(rA) * Math.Pow(Math.Max(0, 27.13 * Math.Abs(rA) / (400 - Math.Abs(rA))), 1 / .42) / ViewingConditions.DEFAULT.rgbD[0];
        double gF = 100 / ViewingConditions.DEFAULT.fl * Math.Sign(gA) * Math.Pow(Math.Max(0, 27.13 * Math.Abs(gA) / (400 - Math.Abs(gA))), 1 / .42) / ViewingConditions.DEFAULT.rgbD[1];
        double bF = 100 / ViewingConditions.DEFAULT.fl * Math.Sign(bA) * Math.Pow(Math.Max(0, 27.13 * Math.Abs(bA) / (400 - Math.Abs(bA))), 1 / .42) / ViewingConditions.DEFAULT.rgbD[2];
        return IntFromXyzComponents(1.86206786 * rF - 1.01125463 * gF + .14918677 * bF, .38752654 * rF + .62144744 * gF - .00897398 * bF, -.0158415 * rF - .03412294 * gF + 1.04996444 * bF);
    }
}