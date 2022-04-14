using MaterialColorUtilities.Palettes;

namespace MaterialColorUtilities.Schemes
{
    public class DarkSchemeMapper : DarkSchemeMapper<CorePalette, Scheme<int>>
    {
    }

    public class DarkSchemeMapper<TCorePalette, TScheme> : BaseSchemeMapper<TCorePalette, TScheme>
        where TCorePalette : CorePalette
        where TScheme : Scheme<int>, new()
    {
        protected override void MapCore(TCorePalette palette, TScheme scheme)
        {
            scheme.Primary = palette.Primary[80];
            scheme.OnPrimary = palette.Primary[20];
            scheme.PrimaryContainer = palette.Primary[30];
            scheme.OnPrimaryContainer = palette.Primary[90];
            scheme.Secondary = palette.Secondary[80];
            scheme.OnSecondary = palette.Secondary[20];
            scheme.SecondaryContainer = palette.Secondary[30];
            scheme.OnSecondaryContainer = palette.Secondary[90];
            scheme.Tertiary = palette.Tertiary[80];
            scheme.OnTertiary = palette.Tertiary[20];
            scheme.TertiaryContainer = palette.Tertiary[30];
            scheme.OnTertiaryContainer = palette.Tertiary[90];
            scheme.Error = palette.Error[80];
            scheme.OnError = palette.Error[20];
            scheme.ErrorContainer = palette.Error[30];
            scheme.OnErrorContainer = palette.Error[90];
            scheme.Background = palette.Neutral[10];
            scheme.OnBackground = palette.Neutral[90];
            scheme.Surface = palette.Neutral[10];
            scheme.OnSurface = palette.Neutral[90];
            scheme.SurfaceVariant = palette.NeutralVariant[30];
            scheme.OnSurfaceVariant = palette.NeutralVariant[80];
            scheme.Outline = palette.NeutralVariant[60];
            scheme.Shadow = palette.Neutral[0];
            scheme.InverseSurface = palette.Neutral[90];
            scheme.InverseOnSurface = palette.Neutral[20];
            scheme.InversePrimary = palette.Primary[40];
        }
    }
}
