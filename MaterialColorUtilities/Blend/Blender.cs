using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Blend;

public static class Blender
{
    /**
     * Blend the design color's HCT hue towards the key color's HCT hue, in a way that leaves the
     * original color recognizable and recognizably shifted towards the key color.
     *
     * @param designColor ARGB representation of an arbitrary color.
     * @param sourceColor ARGB representation of the main theme color.
     * @return The design color with a hue shifted towards the system's color, a slightly
     *     warmer/cooler variant of the design color's hue.
     */
    public static int Harmonize(int designColor, int sourceColor)
    {
        Hct fromHct = Hct.FromInt(designColor);
        Hct toHct = Hct.FromInt(sourceColor);
        double differenceDegrees = MathUtils.DifferenceDegrees(fromHct.Hue, toHct.Hue);
        double rotationDegrees = Math.Min(differenceDegrees * 0.5, 15.0);
        double outputHue =
            MathUtils.SanitizeDegreesDouble(
                fromHct.Hue
                    + rotationDegrees * RotationDirection(fromHct.Hue, toHct.Hue));
        return Hct.From(outputHue, fromHct.Chroma, fromHct.Tone).ToInt();
    }

    /**
     * Blends hue from one color into another. The chroma and tone of the original color are
     * maintained.
     *
     * @param from ARGB representation of color
     * @param to ARGB representation of color
     * @param amount how much blending to perform; 0.0 >= and <= 1.0
     * @return from, with a hue blended towards to. Chroma and tone are constant.
     */
    public static int HctHue(int from, int to, double amount)
    {
        int ucs = Cam16Ucs(from, to, amount);
        Cam16 ucsCam = Cam16.FromInt(ucs);
        Cam16 fromCam = Cam16.FromInt(from);
        return Hct.From(ucsCam.Hue, fromCam.Chroma, ColorUtils.LStarFromArgb(from)).ToInt();
    }

    /**
     * Blend in CAM16-UCS space.
     *
     * @param from ARGB representation of color
     * @param to ARGB representation of color
     * @param amount how much blending to perform; 0.0 >= and <= 1.0
     * @return from, blended towards to. Hue, chroma, and tone will change.
     */
    public static int Cam16Ucs(int from, int to, double amount)
    {
        Cam16 fromCam = Cam16.FromInt(from);
        Cam16 toCam = Cam16.FromInt(to);
        double fromJ = fromCam.Jstar;
        double fromA = fromCam.Astar;
        double fromB = fromCam.Bstar;
        double toJ = toCam.Jstar;
        double toA = toCam.Astar;
        double toB = toCam.Bstar;
        double jstar = fromJ + (toJ - fromJ) * amount;
        double astar = fromA + (toA - fromA) * amount;
        double bstar = fromB + (toB - fromB) * amount;
        return Cam16.FromUcs(jstar, astar, bstar).ToInt();
    }

    /**
     * Sign of direction change needed to travel from one angle to another.
     *
     * @param from The angle travel starts from, in degrees.
     * @param to The angle travel ends at, in degrees.
     * @return -1 if decreasing from leads to the shortest travel distance, 1 if increasing from leads
     *     to the shortest travel distance.
     */
    private static double RotationDirection(double from, double to)
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
}
