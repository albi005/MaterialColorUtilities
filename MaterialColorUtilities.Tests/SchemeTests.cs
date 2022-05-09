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

using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class SchemeTests
    {
        [TestMethod]
        public void BlueLight()
        {
            Scheme<int> scheme = new LightSchemeMapper().Map(new(unchecked((int)0xff0000ff)));
            Assert.AreEqual(unchecked((int)0xff343DFF), scheme.Primary);
        }

        [TestMethod]
        public void BlueDark()
        {
            Scheme<int> scheme = new DarkSchemeMapper().Map(new(unchecked((int)0xff0000ff)));
            Assert.AreEqual(unchecked((int)0xffBEC2FF), scheme.Primary);
        }

        [TestMethod]
        public void PurpleishLight()
        {
            Scheme<int> scheme = new LightSchemeMapper().Map(new(unchecked((int)0xff6750A4)));
            Assert.AreEqual(unchecked((int)0xff6750A4), scheme.Primary);
            Assert.AreEqual(unchecked((int)0xff625B71), scheme.Secondary);
            Assert.AreEqual(unchecked((int)0xff7E5260), scheme.Tertiary);
            Assert.AreEqual(unchecked((int)0xffFFFBFF), scheme.Surface);
            Assert.AreEqual(unchecked((int)0xff1C1B1E), scheme.OnSurface);
        }

        [TestMethod]
        public void PurpleishDark()
        {
            Scheme<int> scheme = new DarkSchemeMapper().Map(new(unchecked((int)0xff6750A4)));
            Assert.AreEqual(unchecked((int)0xffCFBCFF), scheme.Primary);
            Assert.AreEqual(unchecked((int)0xffCBC2DB), scheme.Secondary);
            Assert.AreEqual(unchecked((int)0xffEFB8C8), scheme.Tertiary);
            Assert.AreEqual(unchecked((int)0xff1c1b1e), scheme.Surface);
            Assert.AreEqual(unchecked((int)0xffE6E1E6), scheme.OnSurface);
        }

        [TestMethod]
        public void LightSchemeFromHighChromaColor()
        {
            Scheme<int> scheme = new LightSchemeMapper().Map(new(unchecked((int)0xfffa2bec)));
            Assert.AreEqual(unchecked((int)0xffab00a2), scheme.Primary);
            Assert.AreEqual(unchecked((int)0xffffffff), scheme.OnPrimary);
            Assert.AreEqual(unchecked((int)0xffffd7f3), scheme.PrimaryContainer);
            Assert.AreEqual(unchecked((int)0xff390035), scheme.OnPrimaryContainer);
            Assert.AreEqual(unchecked((int)0xff6e5868), scheme.Secondary);
            Assert.AreEqual(unchecked((int)0xffffffff), scheme.OnSecondary);
            Assert.AreEqual(unchecked((int)0xfff8daee), scheme.SecondaryContainer);
            Assert.AreEqual(unchecked((int)0xff271624), scheme.OnSecondaryContainer);
            Assert.AreEqual(unchecked((int)0xff815343), scheme.Tertiary);
            Assert.AreEqual(unchecked((int)0xffffffff), scheme.OnTertiary);
            Assert.AreEqual(unchecked((int)0xffffdbd0), scheme.TertiaryContainer);
            Assert.AreEqual(unchecked((int)0xff321207), scheme.OnTertiaryContainer);
            Assert.AreEqual(unchecked((int)0xffba1a1a), scheme.Error);
            Assert.AreEqual(unchecked((int)0xffffffff), scheme.OnError);
            Assert.AreEqual(unchecked((int)0xffffdad6), scheme.ErrorContainer);
            Assert.AreEqual(unchecked((int)0xff410002), scheme.OnErrorContainer);
            Assert.AreEqual(unchecked((int)0xfffffbff), scheme.Background);
            Assert.AreEqual(unchecked((int)0xff1f1a1d), scheme.OnBackground);
            Assert.AreEqual(unchecked((int)0xfffffbff), scheme.Surface);
            Assert.AreEqual(unchecked((int)0xff1f1a1d), scheme.OnSurface);
            Assert.AreEqual(unchecked((int)0xffeedee7), scheme.SurfaceVariant);
            Assert.AreEqual(unchecked((int)0xff4e444b), scheme.OnSurfaceVariant);
            Assert.AreEqual(unchecked((int)0xff80747b), scheme.Outline);
            Assert.AreEqual(unchecked((int)0xff000000), scheme.Shadow);
            Assert.AreEqual(unchecked((int)0xff342f32), scheme.InverseSurface);
            Assert.AreEqual(unchecked((int)0xfff8eef2), scheme.InverseOnSurface);
            Assert.AreEqual(unchecked((int)0xffffabee), scheme.InversePrimary);
        }

        [TestMethod]
        public void DarkSchemeFromHighChromaColor()
        {
            Scheme<int> scheme = new DarkSchemeMapper().Map(new(unchecked((int)0xfffa2bec)));
            Assert.AreEqual(unchecked((int)0xffffabee), scheme.Primary);
            Assert.AreEqual(unchecked((int)0xff5c0057), scheme.OnPrimary);
            Assert.AreEqual(unchecked((int)0xff83007b), scheme.PrimaryContainer);
            Assert.AreEqual(unchecked((int)0xffffd7f3), scheme.OnPrimaryContainer);
            Assert.AreEqual(unchecked((int)0xffdbbed1), scheme.Secondary);
            Assert.AreEqual(unchecked((int)0xff3e2a39), scheme.OnSecondary);
            Assert.AreEqual(unchecked((int)0xff554050), scheme.SecondaryContainer);
            Assert.AreEqual(unchecked((int)0xfff8daee), scheme.OnSecondaryContainer);
            Assert.AreEqual(unchecked((int)0xfff5b9a5), scheme.Tertiary);
            Assert.AreEqual(unchecked((int)0xff4c2619), scheme.OnTertiary);
            Assert.AreEqual(unchecked((int)0xff663c2d), scheme.TertiaryContainer);
            Assert.AreEqual(unchecked((int)0xffffdbd0), scheme.OnTertiaryContainer);
            Assert.AreEqual(unchecked((int)0xffffb4ab), scheme.Error);
            Assert.AreEqual(unchecked((int)0xff690005), scheme.OnError);
            Assert.AreEqual(unchecked((int)0xff93000a), scheme.ErrorContainer);
            Assert.AreEqual(unchecked((int)0xffffb4ab), scheme.OnErrorContainer);
            Assert.AreEqual(unchecked((int)0xff1f1a1d), scheme.Background);
            Assert.AreEqual(unchecked((int)0xffeae0e4), scheme.OnBackground);
            Assert.AreEqual(unchecked((int)0xff1f1a1d), scheme.Surface);
            Assert.AreEqual(unchecked((int)0xffeae0e4), scheme.OnSurface);
            Assert.AreEqual(unchecked((int)0xff4e444b), scheme.SurfaceVariant);
            Assert.AreEqual(unchecked((int)0xffd2c2cb), scheme.OnSurfaceVariant);
            Assert.AreEqual(unchecked((int)0xff9a8d95), scheme.Outline);
            Assert.AreEqual(unchecked((int)0xff000000), scheme.Shadow);
            Assert.AreEqual(unchecked((int)0xffeae0e4), scheme.InverseSurface);
            Assert.AreEqual(unchecked((int)0xff342f32), scheme.InverseOnSurface);
            Assert.AreEqual(unchecked((int)0xffab00a2), scheme.InversePrimary);
        }

        [TestMethod]
        public void LightContentSchemeFromHighChromaColor()
        {
            Scheme<int> scheme = new LightSchemeMapper().Map(CorePalette.ContentOf(unchecked((int)0xfffa2bec)));
            Assert.AreEqual(unchecked((int)0xffab00a2), scheme.Primary);
            Assert.AreEqual(unchecked((int)0xffffffff), scheme.OnPrimary);
            Assert.AreEqual(unchecked((int)0xffffd7f3), scheme.PrimaryContainer);
            Assert.AreEqual(unchecked((int)0xff390035), scheme.OnPrimaryContainer);
            Assert.AreEqual(unchecked((int)0xff7f4e75), scheme.Secondary);
            Assert.AreEqual(unchecked((int)0xffffffff), scheme.OnSecondary);
            Assert.AreEqual(unchecked((int)0xffffd7f3), scheme.SecondaryContainer);
            Assert.AreEqual(unchecked((int)0xff330b2f), scheme.OnSecondaryContainer);
            Assert.AreEqual(unchecked((int)0xff9c4323), scheme.Tertiary);
            Assert.AreEqual(unchecked((int)0xffffffff), scheme.OnTertiary);
            Assert.AreEqual(unchecked((int)0xffffdbd0), scheme.TertiaryContainer);
            Assert.AreEqual(unchecked((int)0xff390c00), scheme.OnTertiaryContainer);
            Assert.AreEqual(unchecked((int)0xffba1a1a), scheme.Error);
            Assert.AreEqual(unchecked((int)0xffffffff), scheme.OnError);
            Assert.AreEqual(unchecked((int)0xffffdad6), scheme.ErrorContainer);
            Assert.AreEqual(unchecked((int)0xff410002), scheme.OnErrorContainer);
            Assert.AreEqual(unchecked((int)0xfffffbff), scheme.Background);
            Assert.AreEqual(unchecked((int)0xff1f1a1d), scheme.OnBackground);
            Assert.AreEqual(unchecked((int)0xfffffbff), scheme.Surface);
            Assert.AreEqual(unchecked((int)0xff1f1a1d), scheme.OnSurface);
            Assert.AreEqual(unchecked((int)0xffeedee7), scheme.SurfaceVariant);
            Assert.AreEqual(unchecked((int)0xff4e444b), scheme.OnSurfaceVariant);
            Assert.AreEqual(unchecked((int)0xff80747b), scheme.Outline);
            Assert.AreEqual(unchecked((int)0xff000000), scheme.Shadow);
            Assert.AreEqual(unchecked((int)0xff342f32), scheme.InverseSurface);
            Assert.AreEqual(unchecked((int)0xfff8eef2), scheme.InverseOnSurface);
            Assert.AreEqual(unchecked((int)0xffffabee), scheme.InversePrimary);
        }

        [TestMethod]
        public void DarkContentSchemeFromHighChromaColor()
        {
            Scheme<int> scheme = new DarkSchemeMapper().Map(CorePalette.ContentOf(unchecked((int)0xfffa2bec)));
            Assert.AreEqual(unchecked((int)0xffffabee), scheme.Primary);
            Assert.AreEqual(unchecked((int)0xff5c0057), scheme.OnPrimary);
            Assert.AreEqual(unchecked((int)0xff83007b), scheme.PrimaryContainer);
            Assert.AreEqual(unchecked((int)0xffffd7f3), scheme.OnPrimaryContainer);
            Assert.AreEqual(unchecked((int)0xfff0b4e1), scheme.Secondary);
            Assert.AreEqual(unchecked((int)0xff4b2145), scheme.OnSecondary);
            Assert.AreEqual(unchecked((int)0xff64375c), scheme.SecondaryContainer);
            Assert.AreEqual(unchecked((int)0xffffd7f3), scheme.OnSecondaryContainer);
            Assert.AreEqual(unchecked((int)0xffffb59c), scheme.Tertiary);
            Assert.AreEqual(unchecked((int)0xff5c1900), scheme.OnTertiary);
            Assert.AreEqual(unchecked((int)0xff7d2c0d), scheme.TertiaryContainer);
            Assert.AreEqual(unchecked((int)0xffffdbd0), scheme.OnTertiaryContainer);
            Assert.AreEqual(unchecked((int)0xffffb4ab), scheme.Error);
            Assert.AreEqual(unchecked((int)0xff690005), scheme.OnError);
            Assert.AreEqual(unchecked((int)0xff93000a), scheme.ErrorContainer);
            Assert.AreEqual(unchecked((int)0xffffb4ab), scheme.OnErrorContainer);
            Assert.AreEqual(unchecked((int)0xff1f1a1d), scheme.Background);
            Assert.AreEqual(unchecked((int)0xffeae0e4), scheme.OnBackground);
            Assert.AreEqual(unchecked((int)0xff1f1a1d), scheme.Surface);
            Assert.AreEqual(unchecked((int)0xffeae0e4), scheme.OnSurface);
            Assert.AreEqual(unchecked((int)0xff4e444b), scheme.SurfaceVariant);
            Assert.AreEqual(unchecked((int)0xffd2c2cb), scheme.OnSurfaceVariant);
            Assert.AreEqual(unchecked((int)0xff9a8d95), scheme.Outline);
            Assert.AreEqual(unchecked((int)0xff000000), scheme.Shadow);
            Assert.AreEqual(unchecked((int)0xffeae0e4), scheme.InverseSurface);
            Assert.AreEqual(unchecked((int)0xff342f32), scheme.InverseOnSurface);
            Assert.AreEqual(unchecked((int)0xffab00a2), scheme.InversePrimary);
        }
    }
}
