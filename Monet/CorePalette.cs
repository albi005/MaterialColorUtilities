using System.Drawing;

namespace Monet;

public class CorePalette
{
    public TonalPalette Primary { get; }
    public TonalPalette Secondary { get; private set; }
    public TonalPalette Tertiary { get; private set; }
    public TonalPalette Neutral { get; }
    public TonalPalette NeutralVariant { get; }
    public TonalPalette Error { get; }

    public CorePalette(uint argb)
    {
        Hct hct = Hct.FromInt(argb);
        double hue = hct.Hue;
        Primary = new(hue, Math.Max(48, hct.Chroma));
        Secondary = new(hue, 16);
        Tertiary = new(hue + 60, 24);
        Neutral = new(hue, 4);
        NeutralVariant = new(hue, 8);
        Error = new(25, 84);
    }

    public void SetSecondary(uint argb)
    {
        Hct hct = Hct.FromInt(argb);
        Secondary = new(hct.Hue, Math.Max(48, hct.Chroma));
    }

    public void SetTertiary(uint argb)
    {
        Hct hct = Hct.FromInt(argb);
        Tertiary = new(hct.Hue, Math.Max(48, hct.Chroma));
    }

    public static CorePalette FromColor(Color color) => new((uint)color.ToArgb());
}