namespace MaterialColorUtilities.Quantize;

public interface IPointProvider
{
    public float[] FromInt(int argb);
    public int ToInt(float[] point);
    public float Distance(float[] a, float[] b);
}
