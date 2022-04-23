﻿using MaterialColorUtilities.ColorAppearance;
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
            Assert.AreEqual(unchecked((int)0xff00003c), tones[3]);
            Assert.AreEqual(unchecked((int)0xff00006e), tones[10]);
            Assert.AreEqual(unchecked((int)0xff0001ac), tones[20]);
            Assert.AreEqual(unchecked((int)0xff0000ef), tones[30]);
            Assert.AreEqual(unchecked((int)0xff343dff), tones[40]);
            Assert.AreEqual(unchecked((int)0xff5a64ff), tones[50]);
            Assert.AreEqual(unchecked((int)0xff7c84ff), tones[60]);
            Assert.AreEqual(unchecked((int)0xff9da3ff), tones[70]);
            Assert.AreEqual(unchecked((int)0xffbec2ff), tones[80]);
            Assert.AreEqual(unchecked((int)0xffe0e0ff), tones[90]);
            Assert.AreEqual(unchecked((int)0xfff1efff), tones[95]);
            Assert.AreEqual(unchecked((int)0xfffffbff), tones[99]);
            Assert.AreEqual(unchecked((int)0xffffffff), tones[100]);
        }

        [TestMethod]
        public void CorePalette_OfBlue()
        {
            CorePalette core = CorePalette.Of(unchecked((int)0xff0000ff));
            Assert.AreEqual(unchecked((int)0xff000000), core.Primary[0]);
            Assert.AreEqual(unchecked((int)0xff00003c), core.Primary[3]);
            Assert.AreEqual(unchecked((int)0xff00006e), core.Primary[10]);
            Assert.AreEqual(unchecked((int)0xff0001ac), core.Primary[20]);
            Assert.AreEqual(unchecked((int)0xff0000ef), core.Primary[30]);
            Assert.AreEqual(unchecked((int)0xff343dff), core.Primary[40]);
            Assert.AreEqual(unchecked((int)0xff5a64ff), core.Primary[50]);
            Assert.AreEqual(unchecked((int)0xff7c84ff), core.Primary[60]);
            Assert.AreEqual(unchecked((int)0xff9da3ff), core.Primary[70]);
            Assert.AreEqual(unchecked((int)0xffbec2ff), core.Primary[80]);
            Assert.AreEqual(unchecked((int)0xffe0e0ff), core.Primary[90]);
            Assert.AreEqual(unchecked((int)0xfff1efff), core.Primary[95]);
            Assert.AreEqual(unchecked((int)0xfffffbff), core.Primary[99]);
            Assert.AreEqual(unchecked((int)0xffffffff), core.Primary[100]);
        }
    }
}