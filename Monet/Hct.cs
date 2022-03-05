namespace MaterialColorUtilities;

public class Hct
{
    public double Hue { get; set; }
    public double Chroma { get; set; }
    public double Tone { get; set; }

    public Hct(double hue, double chroma, double tone)
    {
        Hue = hue;
        Chroma = chroma;
        Tone = tone;
        SetInternalState(ToInt());
    }

    private void SetInternalState(uint argb)
    {
        Cam16 cam16 = Cam16.FromIntInViewingConditions(argb);
        double tone = Utils.LStarFromInt(argb);
        Hue = cam16.Hue;
        Chroma = cam16.Chroma;
        Tone = tone;
    }

    public uint ToInt()
        => GetIntInViewingConditions(Utils.SanitizeDegrees(Hue), Chroma, Math.Clamp(Tone, 0, 100));

    public static uint GetIntInViewingConditions(double hue, double chroma, double tone)
    {
        if (1 > chroma || 0 >= Math.Round(tone) || 100 <= Math.Round(tone))
            return Utils.IntFromLStar(tone);

        hue = Utils.SanitizeDegrees(hue);
        double high = chroma;
        double mid = chroma;
        double low = 0;
        bool isFirstLoop = true;
        Cam16? answer = null;
        for (; .4 <= Math.Abs(low - high);)
        {
            var hueInner = hue;
            var chromaInner = mid;
            var toneInner = tone;
            double lowInner = 0; double highInner = 100; double midInner, bestdL = 1E3, bestdE = 1E3; Cam16? bestCam = null;
            for (; .01 < Math.Abs(lowInner - highInner);)
            {
                midInner = lowInner + (highInner - lowInner) / 2;
                uint clipped = Cam16.FromJchInViewingConditions(midInner, chromaInner, hueInner).ToInt();
                double clippedLStar = Utils.LStarFromInt(clipped);
                double dL = Math.Abs(toneInner - clippedLStar);
                if (.2 > dL)
                {
                    Cam16 camClipped = Cam16.FromIntInViewingConditions(clipped);
                    double dE = camClipped.Distance(Cam16.FromJchInViewingConditions(camClipped.J, camClipped.Chroma, hueInner));
                    if (1 >= dE && dE <= bestdE)
                    {
                        bestdL = dL;
                        bestdE = dE;
                        bestCam = camClipped;
                    }
                }
                if (0 == bestdL && 0 == bestdE)
                    break;
                if (clippedLStar < toneInner) lowInner = midInner; else highInner = midInner;
            }
            Cam16? possibleAnswer = bestCam;
            if (isFirstLoop)
            {
                if (null != possibleAnswer)
                    return possibleAnswer.ToInt();
                isFirstLoop = false;
            }
            else
                if (null == possibleAnswer) high = mid;
            else
            {
                answer = possibleAnswer;
                low = mid;
            }
            mid = low + (high - low) / 2;
        }
        return null == answer ? Utils.IntFromLStar(tone) : answer.ToInt();
    }

    public static Hct FromInt(uint argb)
    {
        Cam16 cam = Cam16.FromIntInViewingConditions(argb);
        return new(cam.Hue, cam.Chroma, Utils.LStarFromInt(argb));
    }
}
