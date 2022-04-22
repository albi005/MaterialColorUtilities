using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Palettes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class PalettesTests
    {
        [TestMethod]
        public void TonalPalette_OfBlue()
        {
            Hct hct = Hct.FromInt(unchecked((int)0xff0000ff));
            TonalPalette tones = TonalPalette.FromHueAndChroma(hct.Hue, hct.Chroma);
            Assert.AreEqual(unchecked((int)0xff000000), tones[0]);
            Assert.AreEqual(unchecked((int)0xff00003e), tones[3]);
            Assert.AreEqual(unchecked((int)0xff00006f), tones[10]);
            Assert.AreEqual(unchecked((int)0xff0000ad), tones[20]);
            Assert.AreEqual(unchecked((int)0xff0000f0), tones[30]);
            Assert.AreEqual(unchecked((int)0xff333cff), tones[40]);
            Assert.AreEqual(unchecked((int)0xff5964ff), tones[50]);
            Assert.AreEqual(unchecked((int)0xff7a85ff), tones[60]);
            Assert.AreEqual(unchecked((int)0xff9ca4ff), tones[70]);
            Assert.AreEqual(unchecked((int)0xffbdc2ff), tones[80]);
            Assert.AreEqual(unchecked((int)0xffdfe0ff), tones[90]);
            Assert.AreEqual(unchecked((int)0xfff0efff), tones[95]);
            Assert.AreEqual(unchecked((int)0xfffefbff), tones[99]);
            Assert.AreEqual(unchecked((int)0xffffffff), tones[100]);
        }

        [TestMethod]
        public void CorePalette_OfBlue()
        {
            CorePalette core = CorePalette.Of(unchecked((int)0xff0000ff));
            Assert.AreEqual(unchecked((int)0xff000000), core.Primary[0]);
            Assert.AreEqual(unchecked((int)0xff00003e), core.Primary[3]);
            Assert.AreEqual(unchecked((int)0xff00006f), core.Primary[10]);
            Assert.AreEqual(unchecked((int)0xff0000ad), core.Primary[20]);
            Assert.AreEqual(unchecked((int)0xff0000f0), core.Primary[30]);
            Assert.AreEqual(unchecked((int)0xff333cff), core.Primary[40]);
            Assert.AreEqual(unchecked((int)0xff5964ff), core.Primary[50]);
            Assert.AreEqual(unchecked((int)0xff7a85ff), core.Primary[60]);
            Assert.AreEqual(unchecked((int)0xff9ca4ff), core.Primary[70]);
            Assert.AreEqual(unchecked((int)0xffbdc2ff), core.Primary[80]);
            Assert.AreEqual(unchecked((int)0xffdfe0ff), core.Primary[90]);
            Assert.AreEqual(unchecked((int)0xfff0efff), core.Primary[95]);
            Assert.AreEqual(unchecked((int)0xfffefbff), core.Primary[99]);
            Assert.AreEqual(unchecked((int)0xffffffff), core.Primary[100]);
        }
    }
}
