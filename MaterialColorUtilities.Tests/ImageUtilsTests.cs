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
            uint[] image = Resources.LoadImage("sandy-desert.webp");
            uint resultColor = ImageUtils.ColorsFromImage(image)[0];
            Assert.AreNotEqual(Scorer.Default, resultColor);
        }

        /// <summary>
        /// The image doesn't have any interesting colors
        /// so the result will be the default color.
        /// </summary>
        [TestMethod]
        public void BoringImage()
        {
            uint[] image = Resources.LoadImage("black-rectangle-on-white-background.jpg");
            uint resultColor = ImageUtils.ColorsFromImage(image)[0];
            Assert.AreEqual(Scorer.Default, resultColor);
        }
    }
}