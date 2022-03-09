using MaterialColorUtilities.Quantize;
using MaterialColorUtilities.Score;

namespace MaterialColorUtilities.Utils;

public static class ImageUtils
{
    /// <summary>
    /// Extracts a seed color from an image.
    /// </summary>
    /// <param name="pixels">The colors of the image in ARGB format.</param>
    /// <returns>The seed color.</returns>
    public static int ColorFromImage(int[] pixels)
    {
        var result = QuantizerCelebi.Quantize(pixels, 128);
        var ranked = Scorer.Score(result);
        var top = ranked.First();
        return top;
    }
}
