using MaterialColorUtilities.Quantize;
using MaterialColorUtilities.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class QuantizerWuTests
    {
        private const int Red = unchecked((int)0xffff0000);
        private const int Green = unchecked((int)0xff00ff00);
        private const int Blue = unchecked((int)0xff0000ff);
        private const int Random = unchecked((int)0xff426088);
        private const int MaxColors = 256;

        [TestMethod]
        [DataRow(new[] { Random }, new[] { Random }, DisplayName = "1Random")]
        [DataRow(new[] { Red }, new[] { Red }, DisplayName = "1R")]
        [DataRow(new[] { Green }, new[] { Green }, DisplayName = "1G")]
        [DataRow(new[] { Blue }, new[] { Blue }, DisplayName = "1B")]
        [DataRow(new[] { Blue, Blue, Blue, Blue, Blue }, new[] { Blue }, DisplayName = "5B")]
        [DataRow(new[] { Red, Red, Green, Green, Green }, new[] { Green, Red }, DisplayName = "2R 3G")]
        [DataRow(new[] { Red, Green, Blue }, new[] { Blue, Red, Green }, DisplayName = "1R 1G 1B")]
        public void Quantize(int[] pixels, int[] expected)
        {
            QuantizerWu quantizer = new();
            var result = quantizer.Quantize(pixels, MaxColors);
            int[] colors = result.ColorToCount.Keys.ToArray();
            Assert.AreEqual(expected.Length, colors.Length);
            Assert.That.AreSequenceEqual(expected, colors);
        }
    }
}
