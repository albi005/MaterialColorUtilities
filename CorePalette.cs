using System.Drawing;

namespace Monet;

public class CorePalette
{
    public TonalPalette Primary { get; set; }
    public TonalPalette Secondary { get; set; }
    public TonalPalette Tertiary { get; set; }
    public TonalPalette Neutral { get; set; }
    public TonalPalette NeutralVariant { get; set; }
    public TonalPalette Error { get; set; }

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

    public static CorePalette FromColor(Color color) => new((uint)color.ToArgb());
}