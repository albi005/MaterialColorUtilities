using MaterialColorUtilities.Quantize;
using MaterialColorUtilities.Score;

namespace MaterialColorUtilities.Utils;

public static class ImageUtils
{
    public static int ColorFromImage(int[] image)
    {
        var result = QuantizerCelebi.Quantize(image, 128);
        var ranked = Scorer.Score(result);
        var top = ranked.First();
        return top;
    }
}
