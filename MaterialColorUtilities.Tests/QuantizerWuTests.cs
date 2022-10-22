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

using MaterialColorUtilities.Quantize;
using MaterialColorUtilities.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class QuantizerWuTests
    {
        private const uint Red = 0xffff0000;
        private const uint Green = 0xff00ff00;
        private const uint Blue = 0xff0000ff;
        private const uint Random = 0xff426088;
        private const uint MaxColors = 256;

        [TestMethod]
        [DataRow(new[] { Random }, new[] { Random }, DisplayName = "1Random")]
        [DataRow(new[] { Red }, new[] { Red }, DisplayName = "1R")]
        [DataRow(new[] { Green }, new[] { Green }, DisplayName = "1G")]
        [DataRow(new[] { Blue }, new[] { Blue }, DisplayName = "1B")]
        [DataRow(new[] { Blue, Blue, Blue, Blue, Blue }, new[] { Blue }, DisplayName = "5B")]
        [DataRow(new[] { Red, Red, Green, Green, Green }, new[] { Green, Red }, DisplayName = "2R 3G")]
        [DataRow(new[] { Red, Green, Blue }, new[] { Blue, Red, Green }, DisplayName = "1R 1G 1B")]
        public void Quantize(uint[] pixels, uint[] expected)
        {
            QuantizerWu quantizer = new();
            QuantizerResult result = quantizer.Quantize(pixels, MaxColors);
            uint[] colors = result.ColorToCount.Keys.ToArray();
            Assert.AreEqual(expected.Length, colors.Length);
            Assert.That.AreSequenceEqual(expected, colors);
        }
    }
}
