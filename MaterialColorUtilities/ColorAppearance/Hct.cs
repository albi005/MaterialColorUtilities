using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.ColorAppearance;

public class Hct
{
    private double hue;
    private double chroma;
    private double tone;
    private int argb;

    public static Hct From(double hue, double chroma, double tone)
    {
        int argb = CamSolver.SolveToInt(hue, chroma, tone);
        return new(argb);
    }

    public static Hct FromInt(int argb) => new(argb);

    private Hct(int argb)
    {
        SetInternalState(argb);
    }

    public double Hue
    {
        get => hue;
        set => SetInternalState(CamSolver.SolveToInt(value, chroma, tone));
    }
    public double Chroma
    {
        get => chroma;
        set => SetInternalState(CamSolver.SolveToInt(hue, value, tone));
    }
    public double Tone
    {
        get => tone;
        set => SetInternalState(CamSolver.SolveToInt(hue, chroma, value));
    }

    public int ToInt() => argb;

    private void SetInternalState(int argb)
    {
        this.argb = argb;
        Cam16 cam = Cam16.FromInt(argb);
        hue = cam.Hue;
        chroma = cam.Chroma;
        tone = ColorUtils.LStarFromArgb(argb);
    }
}
