using MaterialColorUtilities.Quantize;
using MaterialColorUtilities.Score;

namespace MaterialColorUtilities.Utils;

public static class ImageUtils
{
    /// <summary>
    /// Extracts colors from an image.
    /// </summary>
    /// <param name="pixels">The colors of the image in ARGB format.</param>
    /// <returns>
    /// The extracted colors in descending order by score.
    /// At least one color will be returned.
    /// </returns>
    public static List<int> ColorsFromImage(int[] pixels)
    {
        var result = QuantizerCelebi.Quantize(pixels, 128);
        var ranked = Scorer.Score(result);
        return ranked;
    }
}
