using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MaterialColorUtilities.Tests.Extensions
{
    public static class AssertExtensions
    {
        public static void IsCloseTo(this Assert assert, double a, double b, double delta)
        {
            double difference = Math.Abs(a - b);
            if (difference > delta) throw new AssertFailedException($"Difference is {difference}.");
        }
    }
}
