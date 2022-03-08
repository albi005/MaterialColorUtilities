using MaterialColorUtilities.Utils;
using System;

namespace MaterialColorUtilities.ColorAppearance;

public class Hct
{
    private float hue;
    private float chroma;
    private float tone;

    public static Hct From(float hue, float chroma, float tone) => new(hue, chroma, tone);

    public static Hct FromInt(int argb)
    {
        Cam16 cam = Cam16.FromInt(argb);
        return new(cam.Hue, cam.Chroma, (float)ColorUtils.LStarFromArgb(argb));
    }

    public Hct(float hue, float chroma, float tone)
    {
        SetInternalState(GamutMap(hue, chroma, tone));
    }

    public float Hue
    {
        get => hue; 
        set => SetInternalState(GamutMap((float)MathUtils.SanitizeDegreesDouble(value), chroma, tone));
    }
    public float Chroma
    {
        get => chroma;
        set => SetInternalState(GamutMap(hue, value, tone));
    }
    public float Tone
    {
        get => tone;
        set => SetInternalState(GamutMap(hue, chroma, value));
    }

    public int ToInt() => GamutMap(hue, chroma, tone);

    private void SetInternalState(int argb)
    {
        Cam16 cam = Cam16.FromInt(argb);
        float tone = (float)ColorUtils.LStarFromArgb(argb);
        hue = cam.Hue;
        chroma = cam.Chroma;
        this.tone = tone;
    }

    /**
     * When the delta between the floor & ceiling of a binary search for maximum chroma at a hue and
     * tone is less than this, the binary search terminates.
     */
    private const float CHROMA_SEARCH_ENDPOINT = 0.4f;

    /** The maximum color distance, in CAM16-UCS, between a requested color and the color returned. */
    private const float DE_MAX = 1.0f;

    /** The maximum difference between the requested L* and the L* returned. */
    private const float DL_MAX = 0.2f;

    /**
     * The minimum color distance, in CAM16-UCS, between a requested color and an 'exact' match. This
     * allows the binary search during gamut mapping to terminate much earlier when the error is
     * infinitesimal.
     */
    private const float DE_MAX_ERROR = 0.000000001f;

    /**
     * When the delta between the floor & ceiling of a binary search for J, lightness in CAM16, is
     * less than this, the binary search terminates.
     */
    private const float LIGHTNESS_SEARCH_ENDPOINT = 0.01f;

    /**
     * @param hue a number, in degrees, representing ex. red, orange, yellow, etc. Ranges from 0 <=
     *     hue < 360.
     * @param chroma Informally, colorfulness. Ranges from 0 to roughly 150. Like all perceptually
     *     accurate color systems, chroma has a different maximum for any given hue and tone, so the
     *     color returned may be lower than the requested chroma.
     * @param tone Lightness. Ranges from 0 to 100.
     * @return ARGB representation of a color in default viewing conditions
     */
    private static int GamutMap(float hue, float chroma, float tone)
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
        float hue, float chroma, float tone, ViewingConditions viewingConditions)
    {

        if (chroma < 1.0 || Math.Round(tone) <= 0.0 || Math.Round(tone) >= 100.0)
        {
            return ColorUtils.ArgbFromLstar(tone);
        }

        hue = (float)MathUtils.SanitizeDegreesDouble(hue);

        float high = chroma;
        float mid = chroma;
        float low = 0.0f;
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
                    mid = low + (high - low) / 2.0f;
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

            mid = low + (high - low) / 2.0f;
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
    private static Cam16 FindCamByJ(float hue, float chroma, float tone)
    {
        float low = 0.0f;
        float high = 100.0f;
        float mid;
        float bestdL = 1000.0f;
        float bestdE = 1000.0f;

        Cam16 bestCam = null;
        while (Math.Abs(low - high) > LIGHTNESS_SEARCH_ENDPOINT)
        {
            mid = low + (high - low) / 2;
            Cam16 camBeforeClip = Cam16.FromJch(mid, chroma, hue);
            int clipped = camBeforeClip.GetInt();
            float clippedLstar = (float)ColorUtils.LStarFromArgb(clipped);
            float dL = Math.Abs(tone - clippedLstar);

            if (dL < DL_MAX)
            {
                Cam16 camClipped = Cam16.FromInt(clipped);
                float dE =
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
