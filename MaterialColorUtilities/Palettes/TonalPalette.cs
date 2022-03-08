using MaterialColorUtilities.ColorAppearance;

namespace MaterialColorUtilities.Palettes;

public class TonalPalette
{
    private readonly Dictionary<int, int> cache = new();
    private readonly float hue;
    private readonly float chroma;

    /**
     * Create tones using the HCT hue and chroma from a color.
     *
     * @param argb ARGB representation of a color
     * @return Tones matching that color's hue and chroma.
     */
    public static TonalPalette FromInt(int argb)
    {
        Hct hct = Hct.FromInt(argb);
        return FromHueAndChroma(hct.Hue, hct.Chroma);
    }

    /**
     * Create tones from a defined HCT hue and chroma.
     *
     * @param hue HCT hue
     * @param chroma HCT chroma
     * @return Tones matching hue and chroma.
     */
    public static TonalPalette FromHueAndChroma(float hue, float chroma) => new(hue, chroma);

    private TonalPalette(float hue, float chroma)
    {
        this.hue = hue;
        this.chroma = chroma;
    }

    /**
     * Create an ARGB color with HCT hue and chroma of this Tones instance, and the provided HCT tone.
     *
     * @param tone HCT tone, measured from 0 to 100.
     * @return ARGB representation of a color with that tone.
     */
    public int Tone(int tone)
        => cache.TryGetValue(tone, out int value)
            ? value
            : cache[tone] = Hct.From(hue, chroma, tone).ToInt();

    public int this[int tone] => Tone(tone);
}
