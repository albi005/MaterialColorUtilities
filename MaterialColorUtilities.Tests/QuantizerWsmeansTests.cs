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
using System;
using System.Collections.Generic;
using System.Linq;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class QuantizerWsmeansTests
    {
        const int Red = unchecked((int)0xFFFF0000);
        const int Green = unchecked((int)0xFF00FF00);
        const int Blue = unchecked((int)0xFF0000FF);
        const int Random = unchecked((int)0xff426088);
        const int MaxColors = 256;

        [TestMethod]
        [DataRow(new[] { Random }, new[] { Random }, DisplayName = "1Rando")]
        [DataRow(new[] { Red }, new[] { Red }, DisplayName = "1R")]
        [DataRow(new[] { Green }, new[] { Green }, DisplayName = "1G")]
        [DataRow(new[] { Blue }, new[] { Blue }, DisplayName = "1B")]
        [DataRow(new[] { Blue, Blue, Blue, Blue, Blue }, new[] { Blue }, DisplayName = "5B")]
        public void Quantize(int[] pixels, int[] expected)
        {
            Dictionary<int, int> result = QuantizerWsmeans.Quantize(pixels, Array.Empty<int>(), MaxColors);
            int[] colors = result.Keys.ToArray();
            Assert.AreEqual(expected.Length, colors.Length);
            Assert.That.AreSequenceEqual(expected, colors);
        }
    }
}
