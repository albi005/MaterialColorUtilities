using MaterialColorUtilities.Blend;
using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;

namespace Playground.Shared;

public partial class AppScheme<TColor> : Scheme<TColor>
{
    public TColor Elevation1 { get; set; }
    public TColor Elevation2 { get; set; }
    public TColor Elevation3 { get; set; }
    public TColor Elevation4 { get; set; }
    public TColor Elevation5 { get; set; }
}

public class DarkAppSchemeMapper : DarkSchemeMapper<CorePalette, AppScheme<int>>
{
    protected override void MapCore(CorePalette corePalette, AppScheme<int> scheme)
    {
        base.MapCore(corePalette, scheme);
        scheme.Elevation1 = Blender.Cam16Ucs(scheme.Background, scheme.Primary, .05);
        scheme.Elevation2 = Blender.Cam16Ucs(scheme.Background, scheme.Primary, .08);
        scheme.Elevation3 = Blender.Cam16Ucs(scheme.Background, scheme.Primary, .11);
        scheme.Elevation4 = Blender.Cam16Ucs(scheme.Background, scheme.Primary, .12);
        scheme.Elevation5 = Blender.Cam16Ucs(scheme.Background, scheme.Primary, .14);
    }
}

public class LightAppSchemeMapper : LightSchemeMapper<CorePalette, AppScheme<int>>
{
    protected override void MapCore(CorePalette corePalette, AppScheme<int> scheme)
    {
        base.MapCore(corePalette, scheme);
        scheme.Elevation1 = Blender.Cam16Ucs(scheme.Background, scheme.Primary, .05);
        scheme.Elevation2 = Blender.Cam16Ucs(scheme.Background, scheme.Primary, .08);
        scheme.Elevation3 = Blender.Cam16Ucs(scheme.Background, scheme.Primary, .11);
        scheme.Elevation4 = Blender.Cam16Ucs(scheme.Background, scheme.Primary, .12);
        scheme.Elevation5 = Blender.Cam16Ucs(scheme.Background, scheme.Primary, .14);
    }
}