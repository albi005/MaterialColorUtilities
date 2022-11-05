using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using MaterialColorUtilities.Utils;

namespace Playground.Wasm;

public partial class MudScheme<TColor> : Scheme<TColor>
{
    public TColor Primary2 { get; set; } = default!;
}

public class DarkMudSchemeMapper : DarkSchemeMapper<CorePalette, MudScheme<uint>>
{
    protected override void MapCore(CorePalette corePalette, MudScheme<uint> scheme)
    {
        base.MapCore(corePalette, scheme);
        scheme.Primary2 = scheme.Primary.Add(scheme.OnPrimary, .08);
    }
}

public class LightMudSchemeMapper : LightSchemeMapper<CorePalette, MudScheme<uint>>
{
    protected override void MapCore(CorePalette corePalette, MudScheme<uint> scheme)
    {
        base.MapCore(corePalette, scheme);
        scheme.Primary2 = scheme.Primary.Add(scheme.OnPrimary, .08);
    }
}