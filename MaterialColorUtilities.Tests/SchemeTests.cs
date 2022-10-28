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
            Scheme<uint> scheme = new LightSchemeMapper().Map(CorePalette.Of(0xff0000ff));
            Assert.AreEqual(0xff343DFF, scheme.Primary);
        }

        [TestMethod]
        public void BlueDark()
        {
            Scheme<uint> scheme = new DarkSchemeMapper().Map(CorePalette.Of(0xff0000ff));
            Assert.AreEqual(0xffBEC2FF, scheme.Primary);
        }

        [TestMethod]
        public void PurpleishLight()
        {
            Scheme<uint> scheme = new LightSchemeMapper().Map(CorePalette.Of(0xff6750A4));
            Assert.AreEqual(0xff6750A4, scheme.Primary);
            Assert.AreEqual(0xff625B71, scheme.Secondary);
            Assert.AreEqual(0xff7E5260, scheme.Tertiary);
            Assert.AreEqual(0xffFFFBFF, scheme.Surface);
            Assert.AreEqual(0xff1C1B1E, scheme.OnSurface);
        }

        [TestMethod]
        public void PurpleishDark()
        {
            Scheme<uint> scheme = new DarkSchemeMapper().Map(CorePalette.Of(0xff6750A4));
            Assert.AreEqual(0xffCFBCFF, scheme.Primary);
            Assert.AreEqual(0xffCBC2DB, scheme.Secondary);
            Assert.AreEqual(0xffEFB8C8, scheme.Tertiary);
            Assert.AreEqual(0xff1c1b1e, scheme.Surface);
            Assert.AreEqual(0xffE6E1E6, scheme.OnSurface);
        }

        [TestMethod]
        public void LightSchemeFromHighChromaColor()
        {
            Scheme<uint> scheme = new LightSchemeMapper().Map(CorePalette.Of(0xfffa2bec));
            Assert.AreEqual(0xffab00a2, scheme.Primary);
            Assert.AreEqual(0xffffffff, scheme.OnPrimary);
            Assert.AreEqual(0xffffd7f3, scheme.PrimaryContainer);
            Assert.AreEqual(0xff390035, scheme.OnPrimaryContainer);
            Assert.AreEqual(0xff6e5868, scheme.Secondary);
            Assert.AreEqual(0xffffffff, scheme.OnSecondary);
            Assert.AreEqual(0xfff8daee, scheme.SecondaryContainer);
            Assert.AreEqual(0xff271624, scheme.OnSecondaryContainer);
            Assert.AreEqual(0xff815343, scheme.Tertiary);
            Assert.AreEqual(0xffffffff, scheme.OnTertiary);
            Assert.AreEqual(0xffffdbd0, scheme.TertiaryContainer);
            Assert.AreEqual(0xff321207, scheme.OnTertiaryContainer);
            Assert.AreEqual(0xffba1a1a, scheme.Error);
            Assert.AreEqual(0xffffffff, scheme.OnError);
            Assert.AreEqual(0xffffdad6, scheme.ErrorContainer);
            Assert.AreEqual(0xff410002, scheme.OnErrorContainer);
            Assert.AreEqual(0xfffffbff, scheme.Background);
            Assert.AreEqual(0xff1f1a1d, scheme.OnBackground);
            Assert.AreEqual(0xfffffbff, scheme.Surface);
            Assert.AreEqual(0xff1f1a1d, scheme.OnSurface);
            Assert.AreEqual(0xffeedee7, scheme.SurfaceVariant);
            Assert.AreEqual(0xff4e444b, scheme.OnSurfaceVariant);
            Assert.AreEqual(0xff80747b, scheme.Outline);
            Assert.AreEqual(0xff000000, scheme.Shadow);
            Assert.AreEqual(0xff342f32, scheme.InverseSurface);
            Assert.AreEqual(0xfff8eef2, scheme.InverseOnSurface);
            Assert.AreEqual(0xffffabee, scheme.InversePrimary);
        }

        [TestMethod]
        public void DarkSchemeFromHighChromaColor()
        {
            Scheme<uint> scheme = new DarkSchemeMapper().Map(CorePalette.Of(0xfffa2bec));
            Assert.AreEqual(0xffffabee, scheme.Primary);
            Assert.AreEqual(0xff5c0057, scheme.OnPrimary);
            Assert.AreEqual(0xff83007b, scheme.PrimaryContainer);
            Assert.AreEqual(0xffffd7f3, scheme.OnPrimaryContainer);
            Assert.AreEqual(0xffdbbed1, scheme.Secondary);
            Assert.AreEqual(0xff3e2a39, scheme.OnSecondary);
            Assert.AreEqual(0xff554050, scheme.SecondaryContainer);
            Assert.AreEqual(0xfff8daee, scheme.OnSecondaryContainer);
            Assert.AreEqual(0xfff5b9a5, scheme.Tertiary);
            Assert.AreEqual(0xff4c2619, scheme.OnTertiary);
            Assert.AreEqual(0xff663c2d, scheme.TertiaryContainer);
            Assert.AreEqual(0xffffdbd0, scheme.OnTertiaryContainer);
            Assert.AreEqual(0xffffb4ab, scheme.Error);
            Assert.AreEqual(0xff690005, scheme.OnError);
            Assert.AreEqual(0xff93000a, scheme.ErrorContainer);
            Assert.AreEqual(0xffffb4ab, scheme.OnErrorContainer);
            Assert.AreEqual(0xff1f1a1d, scheme.Background);
            Assert.AreEqual(0xffeae0e4, scheme.OnBackground);
            Assert.AreEqual(0xff1f1a1d, scheme.Surface);
            Assert.AreEqual(0xffeae0e4, scheme.OnSurface);
            Assert.AreEqual(0xff4e444b, scheme.SurfaceVariant);
            Assert.AreEqual(0xffd2c2cb, scheme.OnSurfaceVariant);
            Assert.AreEqual(0xff9a8d95, scheme.Outline);
            Assert.AreEqual(0xff000000, scheme.Shadow);
            Assert.AreEqual(0xffeae0e4, scheme.InverseSurface);
            Assert.AreEqual(0xff342f32, scheme.InverseOnSurface);
            Assert.AreEqual(0xffab00a2, scheme.InversePrimary);
        }

        [TestMethod]
        public void LightContentSchemeFromHighChromaColor()
        {
            Scheme<uint> scheme = new LightSchemeMapper().Map(CorePalette.ContentOf(0xfffa2bec));
            Assert.AreEqual(0xffab00a2, scheme.Primary);
            Assert.AreEqual(0xffffffff, scheme.OnPrimary);
            Assert.AreEqual(0xffffd7f3, scheme.PrimaryContainer);
            Assert.AreEqual(0xff390035, scheme.OnPrimaryContainer);
            Assert.AreEqual(0xff7f4e75, scheme.Secondary);
            Assert.AreEqual(0xffffffff, scheme.OnSecondary);
            Assert.AreEqual(0xffffd7f3, scheme.SecondaryContainer);
            Assert.AreEqual(0xff330b2f, scheme.OnSecondaryContainer);
            Assert.AreEqual(0xff9c4323, scheme.Tertiary);
            Assert.AreEqual(0xffffffff, scheme.OnTertiary);
            Assert.AreEqual(0xffffdbd0, scheme.TertiaryContainer);
            Assert.AreEqual(0xff390c00, scheme.OnTertiaryContainer);
            Assert.AreEqual(0xffba1a1a, scheme.Error);
            Assert.AreEqual(0xffffffff, scheme.OnError);
            Assert.AreEqual(0xffffdad6, scheme.ErrorContainer);
            Assert.AreEqual(0xff410002, scheme.OnErrorContainer);
            Assert.AreEqual(0xfffffbff, scheme.Background);
            Assert.AreEqual(0xff1f1a1d, scheme.OnBackground);
            Assert.AreEqual(0xfffffbff, scheme.Surface);
            Assert.AreEqual(0xff1f1a1d, scheme.OnSurface);
            Assert.AreEqual(0xffeedee7, scheme.SurfaceVariant);
            Assert.AreEqual(0xff4e444b, scheme.OnSurfaceVariant);
            Assert.AreEqual(0xff80747b, scheme.Outline);
            Assert.AreEqual(0xff000000, scheme.Shadow);
            Assert.AreEqual(0xff342f32, scheme.InverseSurface);
            Assert.AreEqual(0xfff8eef2, scheme.InverseOnSurface);
            Assert.AreEqual(0xffffabee, scheme.InversePrimary);
        }

        [TestMethod]
        public void DarkContentSchemeFromHighChromaColor()
        {
            Scheme<uint> scheme = new DarkSchemeMapper().Map(CorePalette.ContentOf(0xfffa2bec));
            Assert.AreEqual(0xffffabee, scheme.Primary);
            Assert.AreEqual(0xff5c0057, scheme.OnPrimary);
            Assert.AreEqual(0xff83007b, scheme.PrimaryContainer);
            Assert.AreEqual(0xffffd7f3, scheme.OnPrimaryContainer);
            Assert.AreEqual(0xfff0b4e1, scheme.Secondary);
            Assert.AreEqual(0xff4b2145, scheme.OnSecondary);
            Assert.AreEqual(0xff64375c, scheme.SecondaryContainer);
            Assert.AreEqual(0xffffd7f3, scheme.OnSecondaryContainer);
            Assert.AreEqual(0xffffb59c, scheme.Tertiary);
            Assert.AreEqual(0xff5c1900, scheme.OnTertiary);
            Assert.AreEqual(0xff7d2c0d, scheme.TertiaryContainer);
            Assert.AreEqual(0xffffdbd0, scheme.OnTertiaryContainer);
            Assert.AreEqual(0xffffb4ab, scheme.Error);
            Assert.AreEqual(0xff690005, scheme.OnError);
            Assert.AreEqual(0xff93000a, scheme.ErrorContainer);
            Assert.AreEqual(0xffffb4ab, scheme.OnErrorContainer);
            Assert.AreEqual(0xff1f1a1d, scheme.Background);
            Assert.AreEqual(0xffeae0e4, scheme.OnBackground);
            Assert.AreEqual(0xff1f1a1d, scheme.Surface);
            Assert.AreEqual(0xffeae0e4, scheme.OnSurface);
            Assert.AreEqual(0xff4e444b, scheme.SurfaceVariant);
            Assert.AreEqual(0xffd2c2cb, scheme.OnSurfaceVariant);
            Assert.AreEqual(0xff9a8d95, scheme.Outline);
            Assert.AreEqual(0xff000000, scheme.Shadow);
            Assert.AreEqual(0xffeae0e4, scheme.InverseSurface);
            Assert.AreEqual(0xff342f32, scheme.InverseOnSurface);
            Assert.AreEqual(0xffab00a2, scheme.InversePrimary);
        }
    }
}
