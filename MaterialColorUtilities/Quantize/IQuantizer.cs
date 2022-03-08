namespace MaterialColorUtilities.Quantize;

public interface IQuantizer
{
    public QuantizerResult Quantize(int[] pixels, int maxColors);
}
