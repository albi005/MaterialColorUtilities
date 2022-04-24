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
    }
}
