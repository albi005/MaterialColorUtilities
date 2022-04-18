namespace MaterialColorUtilities.Quantize;

public interface IPointProvider
{
    public double[] FromInt(int argb);
    public int ToInt(double[] point);
    public double Distance(double[] a, double[] b);
}
