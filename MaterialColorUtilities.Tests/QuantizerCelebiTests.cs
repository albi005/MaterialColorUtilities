using MaterialColorUtilities.Quantize;
using MaterialColorUtilities.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class QuantizerCelebiTests
    {
        const int Red = unchecked((int)0xFFFF0000);
        const int Green = unchecked((int)0xFF00FF00);
        const int Blue = unchecked((int)0xFF0000FF);
        const int MaxColors = 256;
        
        [TestMethod]
        [DataRow(new[] { Red }, new[] { Red }, DisplayName = "1R")]
        [DataRow(new[] { Green }, new[] { Green }, DisplayName = "1G")]
        [DataRow(new[] { Blue }, new[] { Blue }, DisplayName = "1B")]
        [DataRow(new[] { Blue, Blue, Blue, Blue, Blue },
                 new[] { Blue },
                 DisplayName = "5B")]
        [DataRow(new[] { Red, Green, Blue, },
                 new[] { Blue, Red, Green },
                 DisplayName = "1R 1G 1B")]
        [DataRow(new[] { Red, Red, Green, Green, Green },
                 new[] { Green, Red},
                 DisplayName = "2R 3G")]
        public void Quantize(int[] pixels, int[] expected)
        {
            Dictionary<int, int> result = QuantizerCelebi.Quantize(pixels, MaxColors);
            int[] colors = result.Keys.ToArray();
            Assert.AreEqual(expected.Length, colors.Length);
            Assert.That.AreSequenceEqual(expected, colors);
        }
    }
}
