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

using MaterialColorUtilities.Blend;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class BlenderTests
    {
        const uint Red = 0xffff0000;
        const uint Blue = 0xff0000ff;
        const uint Green = 0xff00ff00;
        const uint Yellow = 0xffffff00;
        
        [TestMethod]
        [DataRow(Red, Blue, 0xffFB0057)]
        [DataRow(Red, Green, 0xffD85600)]
        [DataRow(Red, Yellow, 0xffD85600)]
        [DataRow(Blue, Green, 0xff0047A3)]
        [DataRow(Blue, Red, 0xff5700DC)]
        [DataRow(Blue, Yellow, 0xff0047A3)]
        [DataRow(Green, Blue, 0xff00FC94)]
        [DataRow(Green, Red, 0xffB1F000)]
        [DataRow(Green, Yellow, 0xffB1F000)]
        [DataRow(Yellow, Blue, 0xffEBFFBA)]
        [DataRow(Yellow, Green, 0xffEBFFBA)]
        [DataRow(Yellow, Red, 0xffFFF6E3)]
        public void Harmonize(uint designColor, uint sourceColor, uint result)
        {
            uint answer = Blender.Harmonize(designColor, sourceColor);
            Assert.AreEqual(result, answer);
        }
    }
}
