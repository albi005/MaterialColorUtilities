using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Quantize;

public class PointProviderLab : IPointProvider
{
    public double[] FromInt(int argb)
    {
        double[] lab = ColorUtils.LabFromArgb(argb);
        return new double[] { lab[0], lab[1], lab[2] };
    }

    public int ToInt(double[] lab)
    {
        return ColorUtils.ArgbFromLab(lab[0], lab[1], lab[2]);
    }

    public double Distance(double[] one, double[] two)
    {
        double dL = one[0] - two[0];
        double dA = one[1] - two[1];
        double dB = one[2] - two[2];
        return dL * dL + dA * dA + dB * dB;
    }
}
