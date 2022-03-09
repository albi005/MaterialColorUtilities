using MaterialColorUtilities.Score;
using MaterialColorUtilities.Tests.Utils;
using MaterialColorUtilities.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class ImageUtilsTests
    {
        /// <summary>
        /// Extract a color from an image.
        /// The image has interesting colors so the result shouldn't be the default color.
        /// </summary>
        [TestMethod]
        public void ColorfulImage()
        {
            int[] image = Resources.LoadImage("sandy-desert.webp");
            int resultColor = ImageUtils.ColorFromImage(image);
            Assert.AreNotEqual(Scorer.Default, resultColor);
        }

        /// <summary>
        /// The image doesn't have any interesting colors
        /// so the result will be the default color.
        /// </summary>
        [TestMethod]
        public void BoringImage()
        {
            int[] image = Resources.LoadImage("black-rectangle-on-white-background.jpg");
            int resultColor = ImageUtils.ColorFromImage(image);
            Assert.AreEqual(Scorer.Default, resultColor);
        }
    }
}