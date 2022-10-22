// Copyright 2021 Google LLC
// Copyright 2021-2022 project contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using MaterialColorUtilities.ColorAppearance;
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
        const uint Black = 0xff000000;
        const uint White = 0xffffffff;
        const uint Red = 0xffff0000;
        const uint Green = 0xff00ff00;
        const uint Blue = 0xff0000ff;
        const uint Midgray = 0xff777777;

        [TestMethod]
        public void Conversions_AreReflexive()
        {
            Cam16 cam = Cam16.FromInt(Red);
            uint color = cam.Viewed(ViewingConditions.Default);
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
        public void RgbToCamToRgb(uint colorToTest)
        {
            Cam16 cam = Cam16.FromInt(colorToTest);
            uint color = Hct.From(cam.Hue, cam.Chroma, ColorUtils.LStarFromArgb(colorToTest))
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
            for (uint argb = 0xFF000000; argb < 0xFFFFFFFF - 6969; argb += 6969)
            {
                Hct hct = Hct.FromInt(argb);
                uint reconstructedArgb = Hct.From(hct.Hue, hct.Chroma, hct.Tone).ToInt();
                Assert.AreEqual(argb, reconstructedArgb);
            }
        }

        [TestMethod]
        public void Hct_ReturnsSufficientlyCloseColor()
        {
            for (int hue = 15; hue < 360; hue += 30)
            {
                for (int chroma = 0; chroma <= 100; chroma += 10)
                {
                    for (int tone = 20; tone <= 80; tone += 10)
                    {
                        Hct hctColor = Hct.From(hue, chroma, tone);
                        if (chroma > 0)
                            Assert.That.IsCloseTo(hctColor.Hue, hue, 4.0);
                        Assert.That.IsInInclusiveRange(hctColor.Chroma, 0.0, chroma + 2.5);
                        Assert.That.IsCloseTo(hctColor.Tone, tone, 0.5);
                    }
                }
            }
        }
    }
}
