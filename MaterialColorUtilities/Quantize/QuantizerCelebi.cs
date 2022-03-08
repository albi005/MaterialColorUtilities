namespace MaterialColorUtilities.Quantize;

/**
 * An image quantizer that improves on the quality of a standard K-Means algorithm by setting the
 * K-Means initial state to the output of a Wu quantizer, instead of random centroids. Improves on
 * speed by several optimizations, as implemented in Wsmeans, or Weighted Square Means, K-Means with
 * those optimizations.
 *
 * <p>This algorithm was designed by M. Emre Celebi, and was found in their 2011 paper, Improving
 * the Performance of K-Means for Color Quantization. https://arxiv.org/abs/1101.0395
 */
public static class QuantizerCelebi
{
    public static Dictionary<int, int> Quantize(int[] pixels, int maxColors)
    {
        QuantizerResult wuResult = new QuantizerWu().Quantize(pixels, maxColors);

        ICollection<int> wuClustersAsObjects = wuResult.ColorToCount.Keys;
        int index = 0;
        int[] wuClusters = new int[wuClustersAsObjects.Count];
        foreach (int argb in wuClustersAsObjects)
        {
            wuClusters[index++] = argb;
        }

        return QuantizerWsmeans.Quantize(pixels, wuClusters, maxColors);
    }
}
