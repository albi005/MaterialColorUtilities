﻿using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Tests.Extensions;
using MaterialColorUtilities.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaterialColorUtilities.Tests
{
    /// <summary>
    /// Tests for converting between color spaces.
    /// </summary>
    [TestClass]
    public class HctTests
    {
        const int Black = unchecked((int)0xff000000);
        const int White = unchecked((int)0xffffffff);
        const int Red = unchecked((int)0xffff0000);
        const int Green = unchecked((int)0xff00ff00);
        const int Blue = unchecked((int)0xff0000ff);
        const int Midgray = unchecked((int)0xff777777);

        [TestMethod]
        public void Conversions_AreReflexive()
        {
            Cam16 cam = Cam16.FromInt(Red);
            int color = cam.Viewed(ViewingConditions.Default);
            Assert.AreEqual(color, Red);
        }

        [TestMethod]
        public void Y_Midgray()
        {
            Assert.That.IsCloseTo(18.418, ColorUtils.YFromLstar(50.0), 0.001);
        }

        [TestMethod]
        public void Y_Black()
        {
            Assert.That.IsCloseTo(0.0, ColorUtils.YFromLstar(0.0), 0.001);
        }

        [TestMethod]
        public void Y_White()
        {
            Assert.That.IsCloseTo(100.0, ColorUtils.YFromLstar(100.0), 0.001);
        }
        
        [TestMethod]
        public void RgbToCam_Red()
        {
            Cam16 cam = Cam16.FromInt(Red);
            Assert.That.IsCloseTo(46.445, cam.J, 0.001);
            Assert.That.IsCloseTo(113.357, cam.Chroma, 0.001);
            Assert.That.IsCloseTo(27.408, cam.Hue, 0.001);
            Assert.That.IsCloseTo(89.494, cam.M, 0.001);
            Assert.That.IsCloseTo(91.889, cam.S, 0.001);
            Assert.That.IsCloseTo(105.988, cam.Q, 0.001);
        }

        [TestMethod]
        public void RgbToCam_Green()
        {
            Cam16 cam = Cam16.FromInt(Green);
            Assert.That.IsCloseTo(79.331, cam.J, 0.001);
            Assert.That.IsCloseTo(108.410, cam.Chroma, 0.001);
            Assert.That.IsCloseTo(142.139, cam.Hue, 0.001);
            Assert.That.IsCloseTo(85.587, cam.M, 0.001);
            Assert.That.IsCloseTo(78.604, cam.S, 0.001);
            Assert.That.IsCloseTo(138.520, cam.Q, 0.001);
        }

        [TestMethod]
        public void RgbToCam_Blue()
        {
            Cam16 cam = Cam16.FromInt(Blue);
            Assert.That.IsCloseTo(25.465, cam.J, 0.001);
            Assert.That.IsCloseTo(87.230, cam.Chroma, 0.001);
            Assert.That.IsCloseTo(282.788, cam.Hue, 0.001);
            Assert.That.IsCloseTo(68.867, cam.M, 0.001);
            Assert.That.IsCloseTo(93.674, cam.S, 0.001);
            Assert.That.IsCloseTo(78.481, cam.Q, 0.001);
        }

        [TestMethod]
        public void RgbToCam_Black()
        {
            Cam16 cam = Cam16.FromInt(Black);
            Assert.That.IsCloseTo(0.0, cam.J, 0.001);
            Assert.That.IsCloseTo(0.0, cam.Chroma, 0.001);
            Assert.That.IsCloseTo(0.0, cam.Hue, 0.001);
            Assert.That.IsCloseTo(0.0, cam.M, 0.001);
            Assert.That.IsCloseTo(0.0, cam.S, 0.001);
            Assert.That.IsCloseTo(0.0, cam.Q, 0.001);
        }

        [TestMethod]
        public void RgbToCam_White()
        {
            Cam16 cam = Cam16.FromInt(White);
            Assert.That.IsCloseTo(100.0, cam.J, 0.001);
            Assert.That.IsCloseTo(2.869, cam.Chroma, 0.001);
            Assert.That.IsCloseTo(209.492, cam.Hue, 0.001);
            Assert.That.IsCloseTo(2.265, cam.M, 0.001);
            Assert.That.IsCloseTo(12.068, cam.S, 0.001);
            Assert.That.IsCloseTo(155.521, cam.Q, 0.001);
        }

        [TestMethod]
        [DataRow(Red)]
        [DataRow(Green)]
        [DataRow(Blue)]
        [DataRow(White)]
        [DataRow(Midgray)]
        public void RgbToCamToRgb(int colorToTest)
        {
            Cam16 cam = Cam16.FromInt(colorToTest);
            int color = Hct.From(cam.Hue, cam.Chroma, ColorUtils.LStarFromArgb(colorToTest))
                .ToInt();
            Assert.AreEqual(colorToTest, color);
        }

        [TestMethod]
        public void RgbToHct_Green()
        {
            Hct hct = Hct.FromInt(Green);
            Assert.That.IsCloseTo(142.139, hct.Hue, 2);
            Assert.That.IsCloseTo(108.410, hct.Chroma, 2);
            Assert.That.IsCloseTo(87.737, hct.Tone, 2);
        }

        [TestMethod]
        public void RgbToHct_Blue()
        {
            Hct hct = Hct.FromInt(Blue);
            Assert.That.IsCloseTo(282.788, hct.Hue, 2);
            Assert.That.IsCloseTo(87.230, hct.Chroma, 2);
            Assert.That.IsCloseTo(32.302, hct.Tone, 2);
        }

        [TestMethod]
        public void RgbToHct_BlueTone90()
        {
            Hct hct = Hct.From(282.788, 87.230, 90.0);
            Assert.That.IsCloseTo(280.729, hct.Hue, 2);
            Assert.That.IsCloseTo(19.247, hct.Chroma, 2);
            Assert.That.IsCloseTo(89.961, hct.Tone, 2);
        }

        [TestMethod]
        public void ViewingConditions_Default()
        {
            ViewingConditions vc = ViewingConditions.Default;
            Assert.That.IsCloseTo(vc.N, 0.184, 0.001);
            Assert.That.IsCloseTo(vc.Aw, 29.980, 0.001);
            Assert.That.IsCloseTo(vc.Nbb, 1.016, 0.001);
            Assert.That.IsCloseTo(vc.Ncb, 1.016, 0.001);
            Assert.That.IsCloseTo(vc.C, 0.69, 0.001);
            Assert.That.IsCloseTo(vc.Nc, 1.0, 0.001);
            Assert.That.IsCloseTo(vc.RgbD[0], 1.021, 0.001);
            Assert.That.IsCloseTo(vc.RgbD[1], 0.986, 0.001);
            Assert.That.IsCloseTo(vc.RgbD[2], 0.933, 0.001);
            Assert.That.IsCloseTo(vc.Fl, 0.388, 0.001);
            Assert.That.IsCloseTo(vc.FlRoot, 0.789, 0.001);
            Assert.That.IsCloseTo(vc.Z, 1.909, 0.001);
        }

        [TestMethod]
        public void Hct_PreservesOriginalColor()
        {
            for (int argb = unchecked((int)0xFF000000); argb <= unchecked((int)0xFFFFFFFF); argb += 6969)
            {
                Hct hct = Hct.FromInt(argb);
                int reconstructedArgb = Hct.From(hct.Hue, hct.Chroma, hct.Tone).ToInt();
                Assert.AreEqual(argb, reconstructedArgb);
            }
        }
    }
}
