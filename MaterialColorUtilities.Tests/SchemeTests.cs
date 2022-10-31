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
            Assert.AreEqual(0xff555992, scheme.Primary);
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
            Assert.AreEqual(0xff65558f, scheme.Primary);
            Assert.AreEqual(0xff625b71, scheme.Secondary);
            Assert.AreEqual(0xff7e5260, scheme.Tertiary);
            Assert.AreEqual(0xfffffbff, scheme.Surface);
            Assert.AreEqual(0xff1c1b1e, scheme.OnSurface);
        }

        [TestMethod]
        public void PurpleishDark()
        {
            Scheme<uint> scheme = new DarkSchemeMapper().Map(CorePalette.Of(0xff6750A4));
            Assert.AreEqual(0xffcfbdfe, scheme.Primary);
            Assert.AreEqual(0xffcbc2db, scheme.Secondary);
            Assert.AreEqual(0xffefb8c8, scheme.Tertiary);
            Assert.AreEqual(0xff1c1b1e, scheme.Surface);
            Assert.AreEqual(0xffe6e1e6, scheme.OnSurface);
        }

        [TestMethod]
        public void LightSchemeFromHighChromaColor()
        {
            Scheme<uint> scheme = new LightSchemeMapper().Map(CorePalette.Of(0xfffa2bec));
            Assert.AreEqual(0xff814c77, scheme.Primary);
            Assert.AreEqual(0xffffffff, scheme.OnPrimary);
            Assert.AreEqual(0xffffd7f3, scheme.PrimaryContainer);
            Assert.AreEqual(0xff340830, scheme.OnPrimaryContainer);
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
            Assert.AreEqual(0xfff3b2e4, scheme.InversePrimary);
        }

        [TestMethod]
        public void DarkSchemeFromHighChromaColor()
        {
            Scheme<uint> scheme = new DarkSchemeMapper().Map(CorePalette.Of(0xfffa2bec));
            Assert.AreEqual(0xfff3b2e4, scheme.Primary);
            Assert.AreEqual(0xff4d1f47, scheme.OnPrimary);
            Assert.AreEqual(0xff67355e, scheme.PrimaryContainer);
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
            Assert.AreEqual(0xff814c77, scheme.InversePrimary);
        }

        [TestMethod]
        public void LightContentSchemeFromHighChromaColor()
        {
            Scheme<uint> scheme = new LightSchemeMapper().Map(CorePalette.ContentOf(0xfffa2bec));
            Assert.AreEqual(0xffab00a2, scheme.Primary);
            Assert.AreEqual(0xffffffff, scheme.OnPrimary);
            Assert.AreEqual(0xffffd7f3, scheme.PrimaryContainer);
            Assert.AreEqual(0xff390035, scheme.OnPrimaryContainer);
            Assert.AreEqual(0xff7e4e75, scheme.Secondary);
            Assert.AreEqual(0xffffffff, scheme.OnSecondary);
            Assert.AreEqual(0xffffd7f3, scheme.SecondaryContainer);
            Assert.AreEqual(0xff320b2e, scheme.OnSecondaryContainer);
            Assert.AreEqual(0xff9c3091, scheme.Tertiary);
            Assert.AreEqual(0xffffffff, scheme.OnTertiary);
            Assert.AreEqual(0xffffd7f3, scheme.TertiaryContainer);
            Assert.AreEqual(0xff390035, scheme.OnTertiaryContainer);
            Assert.AreEqual(0xffba1a1a, scheme.Error);
            Assert.AreEqual(0xffffffff, scheme.OnError);
            Assert.AreEqual(0xffffdad6, scheme.ErrorContainer);
            Assert.AreEqual(0xff410002, scheme.OnErrorContainer);
            Assert.AreEqual(0xfffffbff, scheme.Background);
            Assert.AreEqual(0xff22191f, scheme.OnBackground);
            Assert.AreEqual(0xfffffbff, scheme.Surface);
            Assert.AreEqual(0xff22191f, scheme.OnSurface);
            Assert.AreEqual(0xfff9daee, scheme.SurfaceVariant);
            Assert.AreEqual(0xff564050, scheme.OnSurfaceVariant);
            Assert.AreEqual(0xff897081, scheme.Outline);
            Assert.AreEqual(0xff000000, scheme.Shadow);
            Assert.AreEqual(0xff372e34, scheme.InverseSurface);
            Assert.AreEqual(0xfffdecf5, scheme.InverseOnSurface);
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
            Assert.AreEqual(0xffefb4e1, scheme.Secondary);
            Assert.AreEqual(0xff4b2145, scheme.OnSecondary);
            Assert.AreEqual(0xff64375c, scheme.SecondaryContainer);
            Assert.AreEqual(0xffffd7f3, scheme.OnSecondaryContainer);
            Assert.AreEqual(0xffffabee, scheme.Tertiary);
            Assert.AreEqual(0xff5c0057, scheme.OnTertiary);
            Assert.AreEqual(0xff7f1077, scheme.TertiaryContainer);
            Assert.AreEqual(0xffffd7f3, scheme.OnTertiaryContainer);
            Assert.AreEqual(0xffffb4ab, scheme.Error);
            Assert.AreEqual(0xff690005, scheme.OnError);
            Assert.AreEqual(0xff93000a, scheme.ErrorContainer);
            Assert.AreEqual(0xffffb4ab, scheme.OnErrorContainer);
            Assert.AreEqual(0xff22191f, scheme.Background);
            Assert.AreEqual(0xffefdee7, scheme.OnBackground);
            Assert.AreEqual(0xff22191f, scheme.Surface);
            Assert.AreEqual(0xffefdee7, scheme.OnSurface);
            Assert.AreEqual(0xff564050, scheme.SurfaceVariant);
            Assert.AreEqual(0xffdcbed2, scheme.OnSurfaceVariant);
            Assert.AreEqual(0xffa4899b, scheme.Outline);
            Assert.AreEqual(0xff000000, scheme.Shadow);
            Assert.AreEqual(0xffefdee7, scheme.InverseSurface);
            Assert.AreEqual(0xff372e34, scheme.InverseOnSurface);
            Assert.AreEqual(0xffab00a2, scheme.InversePrimary);
        }
    }
}
