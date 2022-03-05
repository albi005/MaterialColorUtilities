namespace MaterialColorUtilities;

public record struct ViewingConditions(
    double N,
    double Aw,
    double Nbb,
    double Ncb,
    double C,
    double Nc,
    double[] RgbD,
    double Fl,
    double FlRoot,
    double Z)
{
    public static ViewingConditions Default { get; } = Create();

    public static ViewingConditions Create(
        double[]? whitePoint = null,
        double adaptingLuminance = 0,
        double backgroundLStar = 50,
        double surround = 2,
        bool discountingIlluminant = false
        )
    {
        if (whitePoint == null)
            whitePoint = Constants.WhitePointD65;
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
        double n = (8 < backgroundLStar ? 100 * Math.Pow((backgroundLStar + 16) / 116, 3) : backgroundLStar / ((double)24389 / 27) * 100) / whitePoint[1];
        double nbb = .725 / Math.Pow(n, .2);
        var rgbAFactors = new double[] { Math.Pow(fl * rgbD[0] * rW / 100, .42), Math.Pow(fl * rgbD[1] * gW / 100, .42), Math.Pow(fl * rgbD[2] * bW / 100, .42) };
        var rgbA = new double[] { 400 * rgbAFactors[0] / (rgbAFactors[0] + 27.13), 400 * rgbAFactors[1] / (rgbAFactors[1] + 27.13), 400 * rgbAFactors[2] / (rgbAFactors[2] + 27.13) };
        return new(n, (2 * rgbA[0] + rgbA[1] + .05 * rgbA[2]) * nbb, nbb, nbb, temp, f, rgbD, fl, Math.Pow(fl, .25), 1.48 + Math.Sqrt(n));
    }
}
