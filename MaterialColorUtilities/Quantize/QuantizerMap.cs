using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Quantize;

public class QuantizerMap : IQuantizer
{
    public Dictionary<int, int> ColorToCount { get; } = new();

    public QuantizerResult Quantize(int[] pixels, int colorCount)
    {
        foreach (int pixel in pixels)
        {
            int alpha = ColorUtils.AlphaFromArgb(pixel);
            if (alpha < 255)
                continue;
            if (ColorToCount.ContainsKey(pixel))
                ColorToCount[pixel]++;
            else
                ColorToCount[pixel] = 1;
        }
        return new(ColorToCount);
    }
}
