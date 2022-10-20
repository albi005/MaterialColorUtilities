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

using MaterialColorUtilities.Tests.Extensions;
using MaterialColorUtilities.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class ColorUtilsTests
    {
        private static double LStarFromY(double y)
        {
            double scaledY = y / 100.0;
            double e = 216.0 / 24389.0;
            if (scaledY <= e)
            {
                return 24389.0 / 27.0 * scaledY;
            }
            else
            {
                double yIntermediate = Math.Pow(scaledY, 1.0 / 3.0);
                return 116.0 * yIntermediate - 16.0;
            }
        }

        private static IEnumerable<double> Range(double start, double stop, int caseCount)
        {
            double stepSize = (stop - start) / (caseCount - 1);
            return Enumerable.Range(0, caseCount).Select(index => start + stepSize * index);
        }

        private static IEnumerable<uint> RgbRange => Enumerable.Range(0, 256 / 8).Select(x => (uint)x * 8);

        private static IEnumerable<uint> FullRgbRange => Enumerable.Range(0, 256).Select(x => (uint)x);

        [TestMethod]
        public void RangeIntegrity()
        {
            List<double> range = Range(3.0, 9999.0, 1234).ToList();
            for (var i = 0; i < 1234; i++)
            {
                Assert.That.IsCloseTo(range[i], 3 + 8.1070559611 * i, 1e-5);
            }
        }

        [TestMethod]
        public void YToLStarToY()
        {
            foreach (var y in Range(0, 100, 1001))
            {
                Assert.That.IsCloseTo(ColorUtils.YFromLstar(LStarFromY(y)), y, 1e-5);
            }
        }

        [TestMethod]
        public void LStarToYToLStar()
        {
            foreach (var lstar in Range(0, 100, 1001))
            {
                Assert.That.IsCloseTo(LStarFromY(ColorUtils.YFromLstar(lstar)), lstar, 1e-5);
            }
        }

        [TestMethod]
        public void YContinuity()
        {
            double epsilon = 1e-6;
            double delta = 1e-8;
            double left = 8.0 - delta;
            double mid = 8.0;
            double right = 8.0 + delta;
            Assert.That.IsCloseTo(
                ColorUtils.YFromLstar(left),
                ColorUtils.YFromLstar(mid),
                epsilon);
            Assert.That.IsCloseTo(
                ColorUtils.YFromLstar(right),
                ColorUtils.YFromLstar(mid),
                epsilon);
        }

        [TestMethod]
        public void RgbToXyzToRgb()
        {
            foreach (uint r in RgbRange)
            {
                foreach (uint g in RgbRange)
                {
                    foreach (uint b in RgbRange)
                    {
                        uint argb = ColorUtils.ArgbFromRgb(r, g, b);
                        double[] xyz = ColorUtils.XyzFromArgb(argb);
                        uint converted = ColorUtils.ArgbFromXyz(xyz[0], xyz[1], xyz[2]);
                        Assert.That.IsCloseTo(ColorUtils.RedFromArgb(converted), r, 1.5);
                        Assert.That.IsCloseTo(ColorUtils.GreenFromArgb(converted), g, 1.5);
                        Assert.That.IsCloseTo(ColorUtils.BlueFromArgb(converted), b, 1.5);
                    }
                }
            }
        }

        [TestMethod]
        public void RgbToLabToRgb()
        {
            foreach (uint r in RgbRange)
            {
                foreach (uint g in RgbRange)
                {
                    foreach (uint b in RgbRange)
                    {
                        uint argb = ColorUtils.ArgbFromRgb(r, g, b);
                        double[] lab = ColorUtils.LabFromArgb(argb);
                        uint converted = ColorUtils.ArgbFromLab(lab[0], lab[1], lab[2]);
                        Assert.That.IsCloseTo(ColorUtils.RedFromArgb(converted), r, 1.5);
                        Assert.That.IsCloseTo(ColorUtils.GreenFromArgb(converted), g, 1.5);
                        Assert.That.IsCloseTo(ColorUtils.BlueFromArgb(converted), b, 1.5);
                    }
                }
            }
        }

        [TestMethod]
        public void RgbToLStarToRgb()
        {
            foreach (uint component in FullRgbRange)
            {
                uint argb = ColorUtils.ArgbFromRgb(component, component, component);
                double lStar = ColorUtils.LStarFromArgb(argb);
                uint converted = ColorUtils.ArgbFromLstar(lStar);
                Assert.AreEqual(converted, argb);
            }
        }

        [TestMethod]
        public void LinearizeDelinearize()
        {
            foreach (uint rgbComponent in FullRgbRange)
            {
                uint converted = ColorUtils.Delinearized(ColorUtils.Linearized(rgbComponent));
                Assert.AreEqual(converted, rgbComponent);
            }
        }
    }
}
