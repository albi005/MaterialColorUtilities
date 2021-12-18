namespace Monet;

public class TonalPalette
{
    private readonly double hue;
    private readonly double chroma;
    private readonly Dictionary<int, uint> cache = new();

    public TonalPalette(double hue, double chroma)
    {
        this.hue = hue;
        this.chroma = chroma;
    }

    public uint Tone(int tone)
    {
        if (!cache.TryGetValue(tone, out uint argb))
            argb = new Hct(hue, chroma, tone).ToInt();
        cache[tone] = argb;
        return argb;
    }

    public uint this[int tone] => Tone(tone);
}
