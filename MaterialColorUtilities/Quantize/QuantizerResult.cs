namespace MaterialColorUtilities.Quantize;

public class QuantizerResult
{
    public Dictionary<int, int> ColorToCount { get; }

    public QuantizerResult(Dictionary<int, int> colorToCount)
    {
        ColorToCount = colorToCount;
    }
}
