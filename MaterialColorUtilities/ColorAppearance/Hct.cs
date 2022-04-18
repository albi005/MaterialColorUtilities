using MaterialColorUtilities.Utils;
using System;

namespace MaterialColorUtilities.ColorAppearance;

public class Hct
{
    private double hue;
    private double chroma;
    private double tone;

    public static Hct From(double hue, double chroma, double tone) => new(hue, chroma, tone);

    public static Hct FromInt(int argb)
    {
        Cam16 cam = Cam16.FromInt(argb);
        return new(cam.Hue, cam.Chroma, ColorUtils.LStarFromArgb(argb));
    }

    public Hct(double hue, double chroma, double tone)
    {
        SetInternalState(GamutMap(hue, chroma, tone));
    }

    public double Hue
    {
        get => hue; 
        set => SetInternalState(GamutMap(MathUtils.SanitizeDegreesDouble(value), chroma, tone));
    }
    public double Chroma
    {
        get => chroma;
        set => SetInternalState(GamutMap(hue, value, tone));
    }
    public double Tone
    {
        get => tone;
        set => SetInternalState(GamutMap(hue, chroma, value));
    }

    public int ToInt() => GamutMap(hue, chroma, tone);

    private void SetInternalState(int argb)
    {
        Cam16 cam = Cam16.FromInt(argb);
        double tone = ColorUtils.LStarFromArgb(argb);
        hue = cam.Hue;
        chroma = cam.Chroma;
        this.tone = tone;
    }

    /**
     * When the delta between the floor & ceiling of a binary search for maximum chroma at a hue and
     * tone is less than this, the binary search terminates.
     */
    private const double CHROMA_SEARCH_ENDPOINT = 0.4;

    /** The maximum color distance, in CAM16-UCS, between a requested color and the color returned. */
    private const double DE_MAX = 1.0;

    /** The maximum difference between the requested L* and the L* returned. */
    private const double DL_MAX = 0.2;

    /**
     * The minimum color distance, in CAM16-UCS, between a requested color and an 'exact' match. This
     * allows the binary search during gamut mapping to terminate much earlier when the error is
     * infinitesimal.
     */
    private const double DE_MAX_ERROR = 0.000000001;

    /**
     * When the delta between the floor & ceiling of a binary search for J, lightness in CAM16, is
     * less than this, the binary search terminates.
     */
    private const double LIGHTNESS_SEARCH_ENDPOINT = 0.01;

    /**
     * @param hue a number, in degrees, representing ex. red, orange, yellow, etc. Ranges from 0 <=
     *     hue < 360.
     * @param chroma Informally, colorfulness. Ranges from 0 to roughly 150. Like all perceptually
     *     accurate color systems, chroma has a different maximum for any given hue and tone, so the
     *     color returned may be lower than the requested chroma.
     * @param tone Lightness. Ranges from 0 to 100.
     * @return ARGB representation of a color in default viewing conditions
     */
    private static int GamutMap(double hue, double chroma, double tone)
    {
        return GamutMapInViewingConditions(hue, chroma, tone, ViewingConditions.Default);
    }

    /**
     * @param hue CAM16 hue.
     * @param chroma CAM16 chroma.
     * @param tone L*a*b* lightness.
     * @param viewingConditions Information about the environment where the color was observed.
     */
    private static int GamutMapInViewingConditions(
        double hue, double chroma, double tone, ViewingConditions viewingConditions)
    {

        if (chroma < 1.0 || Math.Round(tone) <= 0.0 || Math.Round(tone) >= 100.0)
        {
            return ColorUtils.ArgbFromLstar(tone);
        }

        hue = MathUtils.SanitizeDegreesDouble(hue);

        double high = chroma;
        double mid = chroma;
        double low = 0.0;
        bool isFirstLoop = true;

        Cam16 answer = null;
        while (Math.Abs(low - high) >= CHROMA_SEARCH_ENDPOINT)
        {
            Cam16 possibleAnswer = FindCamByJ(hue, mid, tone);

            if (isFirstLoop)
            {
                if (possibleAnswer != null)
                {
                    return possibleAnswer.Viewed(viewingConditions);
                }
                else
                {
                    isFirstLoop = false;
                    mid = low + (high - low) / 2.0;
                    continue;
                }
            }

            if (possibleAnswer == null)
            {
                high = mid;
            }
            else
            {
                answer = possibleAnswer;
                low = mid;
            }

            mid = low + (high - low) / 2.0;
        }

        if (answer == null)
        {
            return ColorUtils.ArgbFromLstar(tone);
        }

        return answer.Viewed(viewingConditions);
    }

    /**
     * @param hue CAM16 hue
     * @param chroma CAM16 chroma
     * @param tone L*a*b* lightness
     * @return CAM16 instance within error tolerance of the provided dimensions, or null.
     */
    private static Cam16 FindCamByJ(double hue, double chroma, double tone)
    {
        double low = 0.0;
        double high = 100.0;
        double mid;
        double bestdL = 1000.0;
        double bestdE = 1000.0;

        Cam16 bestCam = null;
        while (Math.Abs(low - high) > LIGHTNESS_SEARCH_ENDPOINT)
        {
            mid = low + (high - low) / 2;
            Cam16 camBeforeClip = Cam16.FromJch(mid, chroma, hue);
            int clipped = camBeforeClip.GetInt();
            double clippedLstar = ColorUtils.LStarFromArgb(clipped);
            double dL = Math.Abs(tone - clippedLstar);

            if (dL < DL_MAX)
            {
                Cam16 camClipped = Cam16.FromInt(clipped);
                double dE =
                    camClipped.Distance(Cam16.FromJch(camClipped.J, camClipped.Chroma, hue));
                if (dE <= DE_MAX && dE <= bestdE)
                {
                    bestdL = dL;
                    bestdE = dE;
                    bestCam = camClipped;
                }
            }

            if (bestdL == 0 && bestdE < DE_MAX_ERROR)
            {
                break;
            }

            if (clippedLstar < tone)
            {
                low = mid;
            }
            else
            {
                high = mid;
            }
        }

        return bestCam;
    }
}
