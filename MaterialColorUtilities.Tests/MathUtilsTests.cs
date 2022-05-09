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

// Original implementation for MathUtils.rotationDirection.
// Included here to test equivalence with new implementation.
using MaterialColorUtilities.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MaterialColorUtilities.Tests;

[TestClass]
public class MathUtilsTests
{
    private static double RotationDirectionPrev(double from, double to)
    {
        double a = to - from;
        double b = to - from + 360.0;
        double c = to - from - 360.0;
        double aAbs = Math.Abs(a);
        double bAbs = Math.Abs(b);
        double cAbs = Math.Abs(c);
        if (aAbs <= bAbs && aAbs <= cAbs)
        {
            return a >= 0.0 ? 1.0 : -1.0;
        }
        else if (bAbs <= aAbs && bAbs <= cAbs)
        {
            return b >= 0.0 ? 1.0 : -1.0;
        }
        else
        {
            return c >= 0.0 ? 1.0 : -1.0;
        }
    }

    [TestMethod]
    public void RotationDirection_BehavesCorrectly()
    {
        for (double from = 0.0; from < 360.0; from += 15.0)
        {
            for (double to = 7.5; to < 360.0; to += 15.0)
            {
                double expectedAnswer = RotationDirectionPrev(from, to);
                double actualAnswer = MathUtils.RotationDirection(from, to);
                Assert.AreEqual(actualAnswer, expectedAnswer);
                Assert.AreEqual(Math.Abs(actualAnswer), 1.0);
            }
        }
    }
}