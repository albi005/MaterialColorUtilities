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

        private static IEnumerable<int> RgbRange => Enumerable.Range(0, 256 / 8).Select(x => x * 8);

        private static IEnumerable<int> FullRgbRange => Enumerable.Range(0, 256);

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
            foreach (int r in RgbRange)
            {
                foreach (int g in RgbRange)
                {
                    foreach (int b in RgbRange)
                    {
                        int argb = ColorUtils.ArgbFromRgb(r, g, b);
                        double[] xyz = ColorUtils.XyzFromArgb(argb);
                        int converted = ColorUtils.ArgbFromXyz(xyz[0], xyz[1], xyz[2]);
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
            foreach (int r in RgbRange)
            {
                foreach (int g in RgbRange)
                {
                    foreach (int b in RgbRange)
                    {
                        int argb = ColorUtils.ArgbFromRgb(r, g, b);
                        double[] lab = ColorUtils.LabFromArgb(argb);
                        int converted = ColorUtils.ArgbFromLab(lab[0], lab[1], lab[2]);
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
            foreach (int component in FullRgbRange)
            {
                int argb = ColorUtils.ArgbFromRgb(component, component, component);
                double lStar = ColorUtils.LStarFromArgb(argb);
                int converted = ColorUtils.ArgbFromLstar(lStar);
                Assert.AreEqual(converted, argb);
            }
        }

        [TestMethod]
        public void LinearizeDelinearize()
        {
            foreach (int rgbComponent in FullRgbRange)
            {
                int converted = ColorUtils.Delinearized(ColorUtils.Linearized(rgbComponent));
                Assert.AreEqual(converted, rgbComponent);
            }
        }
    }
}
