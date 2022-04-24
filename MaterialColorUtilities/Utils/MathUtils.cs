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

namespace MaterialColorUtilities.Utils;

/// <summary>
/// Utility methods for mathematical operations.
/// </summary>
public class MathUtils
{
    /// <summary>The linear interpolation function.</summary>
    /// <returns>
    /// <paramref name="start"/> if <paramref name="amount"/> = 0 and <paramref name="stop"/> if <paramref name="amount"/> = 1
    /// </returns>
    public static double Lerp(double start, double stop, double amount)
    {
        return (1.0 - amount) * start + amount * stop;
    }

    /// <summary>Clamps an integer between two integers.</summary>
    /// <returns>
    /// <paramref name="input"/> when <paramref name="min"/> ≤ <paramref name="input"/> ≤ <paramref name="max"/>,
    /// and either <paramref name="min"/> or <paramref name="max"/> otherwise.
    /// </returns>
    public static int ClampInt(int min, int max, int input)
    {
        if (input < min)
            return min;
        if (input > max)
            return max;
        return input;
    }

    /// <summary>Clamps an integer between two floating-point numbers.</summary>
    /// <returns>
    /// <paramref name="input"/> when <paramref name="min"/> ≤ <paramref name="input"/> ≤ <paramref name="max"/>,
    /// and either <paramref name="min"/> or <paramref name="max"/> otherwise.
    /// </returns>
    public static double ClampDouble(double min, double max, double input)
    {
        if (input < min)
            return min;
        if (input > max)
            return max;
        return input;
    }

    /// <summary>Sanitizes a degree measure as an integer.</summary>
    /// <returns>A degree measure between 0 (inclusive) and 360 (exclusive).</returns>
    public static int SanitizeDegreesInt(int degrees)
    {
        degrees %= 360;
        if (degrees < 0)
        {
            degrees += 360;
        }
        return degrees;
    }

    /// <summary>Sanitizes a degree measure as a floating-point number.</summary>
    /// <returns>A degree measure between 0.0 (inclusive) and 360.0 (exclusive).</returns>
    public static double SanitizeDegreesDouble(double degrees)
    {
        degrees %= 360.0;
        if (degrees < 0)
        {
            degrees += 360.0;
        }
        return degrees;
    }

    /// <summary>Distance of two points on a circle, represented using degrees.</summary>
    public static double DifferenceDegrees(double a, double b)
    {
        return 180.0 - Math.Abs(Math.Abs(a - b) - 180.0);
    }

    /// <summary>Multiplies a 1x3 row vector with a 3x3 matrix.</summary>
    public static double[] MatrixMultiply(double[] row, double[][] matrix)
    {
        double a = row[0] * matrix[0][0] + row[1] * matrix[0][1] + row[2] * matrix[0][2];
        double b = row[0] * matrix[1][0] + row[1] * matrix[1][1] + row[2] * matrix[1][2];
        double c = row[0] * matrix[2][0] + row[1] * matrix[2][1] + row[2] * matrix[2][2];
        return new double[] { a, b, c };
    }

    public static double ToRadians(double angdeg) => Math.PI / 180 * angdeg;
    public static double Hypot(double x, double y) => Math.Sqrt(x * x + y * y);
    public static double Log1p(double x) => Math.Log(1 + x);
    public static double Expm1(double x) => Math.Exp(x) - 1;
}
