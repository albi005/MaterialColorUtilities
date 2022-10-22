using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;

namespace Playground.Shared;

public partial class AppScheme<TColor> : Scheme<TColor>
{
}

public class DarkAppSchemeMapper : DarkSchemeMapper<CorePalette, AppScheme<uint>>
{
    protected override void MapCore(CorePalette corePalette, AppScheme<uint> scheme)
    {
        base.MapCore(corePalette, scheme);
    }
}

public class LightAppSchemeMapper : LightSchemeMapper<CorePalette, AppScheme<uint>>
{
    protected override void MapCore(CorePalette corePalette, AppScheme<uint> scheme)
    {
        base.MapCore(corePalette, scheme);
    }
}