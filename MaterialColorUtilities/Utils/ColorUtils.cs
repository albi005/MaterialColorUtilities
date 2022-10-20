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
/// Color science utilities.
/// </summary>
/// <remarks>
/// Utility methods for color science constants and color space conversions that aren't HCT or
/// CAM16.
/// </remarks>
public static class ColorUtils
{
    static readonly double[][] SrgbToXyz =
    {
        new[] { 0.41233895, 0.35762064, 0.18051042 },
        new[] { 0.2126, 0.7152, 0.0722 },
        new[] { 0.01932141, 0.11916382, 0.95034478 }
    };

    static readonly double[][] XyzToSrgb =
    {
        new[] { 3.2413774792388685, -1.5376652402851851, -0.49885366846268053 },
        new[] { -0.9691452513005321, 1.8758853451067872, 0.04156585616912061 },
        new[] { 0.05562093689691305, -0.20395524564742123, 1.0571799111220335 }
    };

    /// <summary>
    /// The standard white point; white on a sunny day.
    /// </summary>
    public static double[] WhitePointD65 { get; } = { 95.047, 100, 108.883 };

    /// <summary>Converts a color from RGB components to ARGB format.</summary>
    public static uint ArgbFromRgb(uint red, uint green, uint blue)
    {
        return (255u << 24) | ((red & 255) << 16) | ((green & 255) << 8) | (blue & 255);
    }

    /// <summary>Converts a color from ARGB components to ARGB format.</summary>
    public static uint ArgbFromComponents(uint alpha, uint red, uint green, uint blue)
    {
        return ((alpha & 255) << 24) | ((red & 255) << 16) | ((green & 255) << 8) | (blue & 255);
    }

    /// <summary>Converts a color from linear RGB components to ARGB format.</summary>
    public static uint ArgbFromLinrgb(double[] linrgb)
    {
        uint r = Delinearized(linrgb[0]);
        uint g = Delinearized(linrgb[1]);
        uint b = Delinearized(linrgb[2]);
        return ArgbFromRgb(r, g, b);
    }

    /// <summary>Returns the alpha component of a color in ARGB format.</summary>
    public static uint AlphaFromArgb(uint argb)
    {
        return (argb >> 24) & 255;
    }

    /// <summary>Returns the red component of a color in ARGB format.</summary>
    public static uint RedFromArgb(uint argb)
    {
        return (argb >> 16) & 255;
    }

    /// <summary>Returns the green component of a color in ARGB format.</summary>
    public static uint GreenFromArgb(uint argb)
    {
        return (argb >> 8) & 255;
    }

    /// <summary>Returns the blue component of a color in ARGB format.</summary>
    public static uint BlueFromArgb(uint argb)
    {
        return argb & 255;
    }

    /// <summary>Returns whether a color in ARGB format is opaque.</summary>
    public static bool IsOpaque(uint argb)
    {
        return AlphaFromArgb(argb) >= 255;
    }

    /// <summary>Converts a color from XYZ to ARGB.</summary> 
    public static uint ArgbFromXyz(double x, double y, double z)
    {
        double[][] matrix = XyzToSrgb;
        double linearR = matrix[0][0] * x + matrix[0][1] * y + matrix[0][2] * z;
        double linearG = matrix[1][0] * x + matrix[1][1] * y + matrix[1][2] * z;
        double linearB = matrix[2][0] * x + matrix[2][1] * y + matrix[2][2] * z;
        uint r = Delinearized(linearR);
        uint g = Delinearized(linearG);
        uint b = Delinearized(linearB);
        return ArgbFromRgb(r, g, b);
    }

    /// <summary>Converts a color from ARGB to XYZ.</summary>
    public static double[] XyzFromArgb(uint argb)
    {
        double r = Linearized(RedFromArgb(argb));
        double g = Linearized(GreenFromArgb(argb));
        double b = Linearized(BlueFromArgb(argb));
        return MathUtils.MatrixMultiply(new double[] { r, g, b }, SrgbToXyz);
    }

    /// <summary>Converts a color represented in Lab color space into an ARGB integer.</summary>
    public static uint ArgbFromLab(double l, double a, double b)
    {
        double[] whitePoint = WhitePointD65;
        double fy = (l + 16.0) / 116.0;
        double fx = a / 500.0 + fy;
        double fz = fy - b / 200.0;
        double xNormalized = LabInvf(fx);
        double yNormalized = LabInvf(fy);
        double zNormalized = LabInvf(fz);
        double x = xNormalized * whitePoint[0];
        double y = yNormalized * whitePoint[1];
        double z = zNormalized * whitePoint[2];
        return ArgbFromXyz(x, y, z);
    }

    /// <summary>Converts a color from ARGB representation to L*a*b* representation.</summary>
    public static double[] LabFromArgb(uint argb)
    {
        double linearR = Linearized(RedFromArgb(argb));
        double linearG = Linearized(GreenFromArgb(argb));
        double linearB = Linearized(BlueFromArgb(argb));
        double[][] matrix = SrgbToXyz;
        double x = matrix[0][0] * linearR + matrix[0][1] * linearG + matrix[0][2] * linearB;
        double y = matrix[1][0] * linearR + matrix[1][1] * linearG + matrix[1][2] * linearB;
        double z = matrix[2][0] * linearR + matrix[2][1] * linearG + matrix[2][2] * linearB;
        double[] whitePoint = WhitePointD65;
        double xNormalized = x / whitePoint[0];
        double yNormalized = y / whitePoint[1];
        double zNormalized = z / whitePoint[2];
        double fx = LabF(xNormalized);
        double fy = LabF(yNormalized);
        double fz = LabF(zNormalized);
        double l = 116.0 * fy - 16;
        double a = 500.0 * (fx - fy);
        double b = 200.0 * (fy - fz);
        return new double[] { l, a, b };
    }

    /// <summary>Converts an L* value to an ARGB representation.</summary>
    /// <param name="l">L* in L*a*b*</param>
    /// <returns>ARGB representation of grayscale color with lightness matching L*</returns>
    public static uint ArgbFromLstar(double lstar)
    {
        double fy = (lstar + 16.0) / 116.0;
        double fz = fy;
        double fx = fy;
        double kappa = 24389.0 / 27.0;
        double epsilon = 216.0 / 24389.0;
        bool lExceedsEpsilonKappa = lstar > 8.0;
        double y = lExceedsEpsilonKappa ? fy * fy * fy : lstar / kappa;
        bool cubeExceedEpsilon = fy * fy * fy > epsilon;
        double x = cubeExceedEpsilon ? fx * fx * fx : lstar / kappa;
        double z = cubeExceedEpsilon ? fz * fz * fz : lstar / kappa;
        double[] whitePoint = WhitePointD65;
        return ArgbFromXyz(x * whitePoint[0], y * whitePoint[1], z * whitePoint[2]);
    }

    /// <summary>Computes the L* value of a color in ARGB representation.</summary>
    /// <param name="argb">ARGB representation of a color</param>
    /// <returns>L*, from L*a*b*, coordinate of the color</returns>
    public static double LStarFromArgb(uint argb)
    {
        double y = XyzFromArgb(argb)[1] / 100.0;
        double e = 216.0 / 24389.0;
        if (y <= e)
        {
            return 24389.0 / 27.0 * y;
        }
        else
        {
            double yIntermediate = Math.Pow(y, 1.0 / 3.0);
            return 116.0 * yIntermediate - 16.0;
        }
    }

    /// <summary>
    /// Converts an L* value to a Y value.
    /// </summary>
    /// <remarks>
    /// L* in L*a*b* and Y in XYZ measure the same quantity, luminance.
    /// L* measures perceptual luminance, a linear scale. Y in XYZ measures relative luminance, a
    /// logarithmic scale.
    /// </remarks>
    /// <param name="lstar">L* in L*a*b*</param>
    /// <returns>Y in XYZ</returns>
    public static double YFromLstar(double lstar)
    {
        double ke = 8.0;
        if (lstar > ke)
        {
            return Math.Pow((lstar + 16.0) / 116.0, 3.0) * 100.0;
        }
        else
        {
            return lstar / (24389.0 / 27.0) * 100.0;
        }
    }

    /// <summary>
    /// Linearizes an RGB component.
    /// </summary>
    /// <param name="rgbComponent">0 ≤ rgb_component ≤ 255, represents R/G/B channel</param>
    /// <returns>0.0 ≤ output ≤ 100.0, color channel converted to linear RGB space</returns>
    public static double Linearized(uint rgbComponent)
    {
        double normalized = rgbComponent / 255.0;
        if (normalized <= 0.040449936)
        {
            return normalized / 12.92 * 100.0;
        }
        else
        {
            return Math.Pow((normalized + 0.055) / 1.055, 2.4) * 100.0;
        }
    }

    /// <summary>
    /// Delinearizes an RGB component.
    /// </summary>
    /// <param name="rgbComponent">0.0 ≤ rgb_component ≤ 100.0, represents linear R/G/B channel</param>
    /// <returns>0 ≤ output ≤ 255, color channel converted to regular RGB space</returns>
    public static uint Delinearized(double rgbComponent)
    {
        double normalized = rgbComponent / 100.0;
        double delinearized;
        if (normalized <= 0.0031308)
        {
            delinearized = normalized * 12.92;
        }
        else
        {
            delinearized = 1.055 * Math.Pow(normalized, 1.0 / 2.4) - 0.055;
        }
        return (uint)MathUtils.ClampInt(0, 255, (int)Math.Round(delinearized * 255.0));
    }

    public static double LabF(double t)
    {
        const double e = 216.0 / 24389.0;
        const double kappa = 24389.0 / 27.0;
        if (t > e)
        {
            return Math.Pow(t, 1.0 / 3.0);
        }
        else
        {
            return (kappa * t + 16) / 116;
        }
    }

    public static double LabInvf(double ft)
    {
        double e = 216.0 / 24389.0;
        double kappa = 24389.0 / 27.0;
        double ft3 = ft * ft * ft;
        if (ft3 > e)
        {
            return ft3;
        }
        else
        {
            return (116 * ft - 16) / kappa;
        }
    }

    public static uint Add(this uint background, uint foreground, double foregroundAlpha)
    {
        uint a = (uint)(foregroundAlpha * 255);
        foreground &= (a << 24) | 0x00FFFFFF;
        return Add(background, foreground);
    }

    public static uint Add(this uint background, uint foreground)
    {
        DeconstructArgb(background,
            out float bgA,
            out float bgR,
            out float bgG,
            out float bgB);
        DeconstructArgb(foreground,
            out float fgA,
            out float fgR,
            out float fgG,
            out float fgB);

        float a = fgA + (bgA * (1 - fgA));

        float r = CompositeComponent(fgR, bgR, fgA, bgA, a);
        float g = CompositeComponent(fgG, bgG, fgA, bgA, a);
        float b = CompositeComponent(fgB, bgB, fgA, bgA, a);

        return ArgbFromComponents(
            (uint)(a * 255),
            (uint)(r * 255),
            (uint)(g * 255),
            (uint)(b * 255));
    }

    public static float CompositeComponent(float fgC, float bgC, float fgA, float bgA, float a)
    {
        if (a == 0) return 0;
        return ((fgC * fgA) + (bgC * bgA * (1 - fgA))) / a;
    }

    public static void DeconstructArgb(
        uint argb,
        out float a,
        out float r,
        out float g,
        out float b)
    {
        a = AlphaFromArgb(argb) / 255f;
        r = RedFromArgb(argb) / 255f;
        g = GreenFromArgb(argb) / 255f;
        b = BlueFromArgb(argb) / 255f;
    }
}
