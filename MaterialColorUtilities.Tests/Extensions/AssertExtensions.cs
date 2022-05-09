using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MaterialColorUtilities.Tests.Extensions
{
    public static class AssertExtensions
    {
        public static void IsCloseTo(this Assert assert, double a, double b, double delta)
        {
            double difference = Math.Abs(a - b);
            if (difference > delta)
                throw new AssertFailedException($"Difference is {difference}.");
        }

        public static void IsInInclusiveRange(this Assert assert, double value, double low, double high)
        {
            if (value < low || value > high)
                throw new AssertFailedException($"Value is not in range [{low}, {high}].");
        }

        public static void AreSequenceEqual<T>(this Assert assert, IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (!expected.SequenceEqual(actual))
                throw new AssertFailedException($"Sequences are not equal.");
        }
    }
}
