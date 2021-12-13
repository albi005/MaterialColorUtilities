namespace Monet;

public record Cam16(
    double Hue,
    double Chroma,
    double J,
    double Q,
    double S,
    double JStar,
    double AStar,
    double BStar)
{
    public double Distance(Cam16 other)
    {
        double dJ = JStar - other.JStar;
        double dA = AStar - other.AStar;
        double dB = BStar - other.BStar;
        return 1.41 * Math.Pow(Math.Sqrt(dJ * dJ + dA * dA + dB * dB), .63);
    }

    public static Cam16 FromJchInViewingConditions(double j, double c, double h)
    {
        double hueRadians = h * Math.PI / 180;
        double mStar = 1 / .0228 * Math.Log(1 + .0228 * c * ViewingConditions.Default.FlRoot);
        return new(
            h,
            c,
            j,
            4 / ViewingConditions.Default.C * Math.Sqrt(j / 100) * (ViewingConditions.Default.Aw + 4) * ViewingConditions.Default.FlRoot,
            50 * Math.Sqrt(c / Math.Sqrt(j / 100) * ViewingConditions.Default.C / (ViewingConditions.Default.Aw + 4)),
            (1 + 100 * .007) * j / (1 + .007 * j),
            mStar * Math.Cos(hueRadians),
            mStar * Math.Sin(hueRadians));
    }

    public static Cam16 FromIntInViewingConditions(uint argb)
    {
        var redL = 100 * Utils.Linearized((double)((argb & 16711680) >> 16) / 255);
        var greenL = 100 * Utils.Linearized((double)((argb & 65280) >> 8) / 255);
        var blueL = 100 * Utils.Linearized((double)(argb & 255) / 255);
        var x = .41233895 * redL + .35762064 * greenL + .18051042 * blueL;
        var y = .2126 * redL + .7152 * greenL + .0722 * blueL;
        var z = .01932141 * redL + .11916382 * greenL + .95034478 * blueL;
        var rD = ViewingConditions.Default.RgbD[0] * (.401288 * x + .650173 * y - .051461 * z);
        var gD = ViewingConditions.Default.RgbD[1] * (-.250268 * x + 1.204414 * y + .045854 * z);
        var bD = ViewingConditions.Default.RgbD[2] * (-.002079 * x + .048952 * y + .953127 * z);
        var rAF = Math.Pow(ViewingConditions.Default.Fl * Math.Abs(rD) / 100, .42);
        var gAF = Math.Pow(ViewingConditions.Default.Fl * Math.Abs(gD) / 100, .42);
        var bAF = Math.Pow(ViewingConditions.Default.Fl * Math.Abs(bD) / 100, .42);
        var rA = 400 * Math.Sign(rD) * rAF / (rAF + 27.13);
        var gA = 400 * Math.Sign(gD) * gAF / (gAF + 27.13);
        var bA = 400 * Math.Sign(bD) * bAF / (bAF + 27.13);
        var a = (11 * rA + -12 * gA + bA) / 11;
        var b = (rA + gA - 2 * bA) / 9;
        var atanDegrees = 180 * Math.Atan2(b, a) / Math.PI;
        var hue = 0 > atanDegrees ? atanDegrees + 360 : 360 <= atanDegrees ? atanDegrees - 360 : atanDegrees;
        var hueRadians = hue * Math.PI / 180;
        var j = 100 * Math.Pow((40 * rA + 20 * gA + bA) / 20 * ViewingConditions.Default.Nbb / ViewingConditions.Default.Aw, ViewingConditions.Default.C * ViewingConditions.Default.Z);
        var alpha = Math.Pow(5E4 / 13 * .25 * (Math.Cos((20.14 > hue ? hue + 360 : hue) * Math.PI / 180 + 2) + 3.8) * ViewingConditions.Default.Nc * ViewingConditions.Default.Ncb * Math.Sqrt(a * a + b * b) / ((20 * rA + 20 * gA + 21 * bA) / 20 + .305), .9) * Math.Pow(1.64 - Math.Pow(.29, ViewingConditions.Default.N), .73);
        var c = alpha * Math.Sqrt(j / 100);
        var mStar = 1 / .0228 * Math.Log(1 + .0228 * c * ViewingConditions.Default.FlRoot);
        return new(
            hue,
            c,
            j,
            4 / ViewingConditions.Default.C * Math.Sqrt(j / 100) * (ViewingConditions.Default.Aw + 4) * ViewingConditions.Default.FlRoot,
            50 * Math.Sqrt(alpha * ViewingConditions.Default.C / (ViewingConditions.Default.Aw + +4)),
            (1 + 100 * .007) * j / (1 + .007 * j),
            mStar * Math.Cos(hueRadians),
            mStar * Math.Sin(hueRadians));
    }

    public uint ToInt()
    {
        double t = Math.Pow((0 == Chroma || 0 == J ? 0 : Chroma / Math.Sqrt(J / 100)) / Math.Pow(1.64 - Math.Pow(.29, ViewingConditions.Default.N), .73), 1 / .9);
        double hRad = Hue * Math.PI / 180;
        double p2 = ViewingConditions.Default.Aw * Math.Pow(J / 100, 1 / ViewingConditions.Default.C / ViewingConditions.Default.Z) / ViewingConditions.Default.Nbb;
        double hSin = Math.Sin(hRad);
        double hCos = Math.Cos(hRad);
        double gamma = 23 * (p2 + .305) * t / (5E4 / 13 * (Math.Cos(hRad + 2) + 3.8) * 5.75 * ViewingConditions.Default.Nc * ViewingConditions.Default.Ncb + 11 * t * hCos + 108 * t * hSin);
        double a = gamma * hCos;
        double b = gamma * hSin;
        double rA = (460 * p2 + 451 * a + 288 * b) / 1403;
        double gA = (460 * p2 - 891 * a - 261 * b) / 1403;
        double bA = (460 * p2 - 220 * a - 6300 * b) / 1403;
        double rF = 100 / ViewingConditions.Default.Fl * Math.Sign(rA) * Math.Pow(Math.Max(0, 27.13 * Math.Abs(rA) / (400 - Math.Abs(rA))), 1 / .42) / ViewingConditions.Default.RgbD[0];
        double gF = 100 / ViewingConditions.Default.Fl * Math.Sign(gA) * Math.Pow(Math.Max(0, 27.13 * Math.Abs(gA) / (400 - Math.Abs(gA))), 1 / .42) / ViewingConditions.Default.RgbD[1];
        double bF = 100 / ViewingConditions.Default.Fl * Math.Sign(bA) * Math.Pow(Math.Max(0, 27.13 * Math.Abs(bA) / (400 - Math.Abs(bA))), 1 / .42) / ViewingConditions.Default.RgbD[2];
        return Utils.IntFromXyzComponents(1.86206786 * rF - 1.01125463 * gF + .14918677 * bF, .38752654 * rF + .62144744 * gF - .00897398 * bF, -.0158415 * rF - .03412294 * gF + 1.04996444 * bF);
    }

}
