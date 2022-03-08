using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Quantize;

public class PointProviderLab : IPointProvider
{
    public float[] FromInt(int argb)
    {
        double[] lab = ColorUtils.LabFromArgb(argb);
        return new float[] { (float)lab[0], (float)lab[1], (float)lab[2] };
    }

    public int ToInt(float[] lab)
    {
        return ColorUtils.ArgbFromLab(lab[0], lab[1], lab[2]);
    }

    public float Distance(float[] one, float[] two)
    {
        float dL = one[0] - two[0];
        float dA = one[1] - two[1];
        float dB = one[2] - two[2];
        return dL * dL + dA * dA + dB * dB;
    }
}
