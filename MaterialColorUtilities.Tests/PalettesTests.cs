﻿// Copyright 2021 Google LLC
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
            Hct hct = Hct.FromInt(0xff0000ff);
            TonalPalette tones = TonalPalette.FromHueAndChroma(hct.Hue, hct.Chroma);
            Assert.AreEqual(0xff000000, tones[0]);
            Assert.AreEqual(0xff00003c, tones[3]);
            Assert.AreEqual(0xff00006e, tones[10]);
            Assert.AreEqual(0xff0001ac, tones[20]);
            Assert.AreEqual(0xff0000ef, tones[30]);
            Assert.AreEqual(0xff343dff, tones[40]);
            Assert.AreEqual(0xff5a64ff, tones[50]);
            Assert.AreEqual(0xff7c84ff, tones[60]);
            Assert.AreEqual(0xff9da3ff, tones[70]);
            Assert.AreEqual(0xffbec2ff, tones[80]);
            Assert.AreEqual(0xffe0e0ff, tones[90]);
            Assert.AreEqual(0xfff1efff, tones[95]);
            Assert.AreEqual(0xfffffbff, tones[99]);
            Assert.AreEqual(0xffffffff, tones[100]);
        }

        [TestMethod]
        public void CorePalette_Of_Blue()
        {
            CorePalette core = CorePalette.Of(0xff0000ff);
            Assert.AreEqual(0xffffffff, core.Primary[100]);
            Assert.AreEqual(0xfff1efff, core.Primary[95]);
            Assert.AreEqual(0xffe0e0ff, core.Primary[90]);
            Assert.AreEqual(0xffbec2ff, core.Primary[80]);
            Assert.AreEqual(0xffa3a6e4, core.Primary[70]);
            Assert.AreEqual(0xff888cc8, core.Primary[60]);
            Assert.AreEqual(0xff6e72ad, core.Primary[50]);
            Assert.AreEqual(0xff555992, core.Primary[40]);
            Assert.AreEqual(0xff3e4278, core.Primary[30]);
            Assert.AreEqual(0xff272b60, core.Primary[20]);
            Assert.AreEqual(0xff11144b, core.Primary[10]);
            Assert.AreEqual(0xff000000, core.Primary[0]);
            Assert.AreEqual(0xffffffff, core.Secondary[100]);
            Assert.AreEqual(0xfff1efff, core.Secondary[95]);
            Assert.AreEqual(0xffe1e0f9, core.Secondary[90]);
            Assert.AreEqual(0xffc5c4dd, core.Secondary[80]);
            Assert.AreEqual(0xffa9a9c1, core.Secondary[70]);
            Assert.AreEqual(0xff8f8fa6, core.Secondary[60]);
            Assert.AreEqual(0xff75758b, core.Secondary[50]);
            Assert.AreEqual(0xff5c5d72, core.Secondary[40]);
            Assert.AreEqual(0xff444559, core.Secondary[30]);
            Assert.AreEqual(0xff2e2f42, core.Secondary[20]);
            Assert.AreEqual(0xff191a2c, core.Secondary[10]);
            Assert.AreEqual(0xff000000, core.Secondary[0]);
        }

        [TestMethod]
        public void CorePalette_ContentOf_Blue()
        {
            CorePalette core = CorePalette.ContentOf(0xff0000ff);
            Assert.AreEqual(0xffffffff, core.Primary[100]);
            Assert.AreEqual(0xfff1efff, core.Primary[95]);
            Assert.AreEqual(0xffe0e0ff, core.Primary[90]);
            Assert.AreEqual(0xffbec2ff, core.Primary[80]);
            Assert.AreEqual(0xff9da3ff, core.Primary[70]);
            Assert.AreEqual(0xff7c84ff, core.Primary[60]);
            Assert.AreEqual(0xff5a64ff, core.Primary[50]);
            Assert.AreEqual(0xff343dff, core.Primary[40]);
            Assert.AreEqual(0xff0000ef, core.Primary[30]);
            Assert.AreEqual(0xff0001ac, core.Primary[20]);
            Assert.AreEqual(0xff00006e, core.Primary[10]);
            Assert.AreEqual(0xff000000, core.Primary[0]);
            Assert.AreEqual(0xffffffff, core.Secondary[100]);
            Assert.AreEqual(0xfff1efff, core.Secondary[95]);
            Assert.AreEqual(0xffe0e0ff, core.Secondary[90]);
            Assert.AreEqual(0xffc1c3f4, core.Secondary[80]);
            Assert.AreEqual(0xffa5a7d7, core.Secondary[70]);
            Assert.AreEqual(0xff8b8dbb, core.Secondary[60]);
            Assert.AreEqual(0xff7174a0, core.Secondary[50]);
            Assert.AreEqual(0xff585b85, core.Secondary[40]);
            Assert.AreEqual(0xff40436c, core.Secondary[30]);
            Assert.AreEqual(0xff2a2d54, core.Secondary[20]);
            Assert.AreEqual(0xff15173e, core.Secondary[10]);
            Assert.AreEqual(0xff000000, core.Secondary[0]);
        }        
    }
}
