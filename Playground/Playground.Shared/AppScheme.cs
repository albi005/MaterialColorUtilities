using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;

namespace Playground.Shared;

public partial class AppScheme<TColor> : Scheme<TColor>
{
}

public class DarkAppSchemeMapper : DarkSchemeMapper<CorePalette, AppScheme<int>>
{
    protected override void MapCore(CorePalette corePalette, AppScheme<int> scheme)
    {
        base.MapCore(corePalette, scheme);
    }
}

public class LightAppSchemeMapper : LightSchemeMapper<CorePalette, AppScheme<int>>
{
    protected override void MapCore(CorePalette corePalette, AppScheme<int> scheme)
    {
        base.MapCore(corePalette, scheme);
    }
}