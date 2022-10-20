using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Schemes
{
    /// <inheritdoc/>
    public class DarkSchemeMapper : DarkSchemeMapper<CorePalette, Scheme<uint>>
    {
    }

    /// <summary>
    /// A mapper that maps a core palette to a dark scheme using the 
    /// <see href="https://m3.material.io/styles/color/the-color-system/tokens">default Material Design 3 mappings.</see>
    /// </summary>
    public class DarkSchemeMapper<TCorePalette, TScheme> : BaseSchemeMapper<TCorePalette, TScheme>
        where TCorePalette : CorePalette
        where TScheme : Scheme<uint>, new()
    {
        protected override void MapCore(TCorePalette corePalette, TScheme scheme)
        {
            scheme.Primary = corePalette.Primary[80];
            scheme.OnPrimary = corePalette.Primary[20];
            scheme.PrimaryContainer = corePalette.Primary[30];
            scheme.OnPrimaryContainer = corePalette.Primary[90];
            scheme.Secondary = corePalette.Secondary[80];
            scheme.OnSecondary = corePalette.Secondary[20];
            scheme.SecondaryContainer = corePalette.Secondary[30];
            scheme.OnSecondaryContainer = corePalette.Secondary[90];
            scheme.Tertiary = corePalette.Tertiary[80];
            scheme.OnTertiary = corePalette.Tertiary[20];
            scheme.TertiaryContainer = corePalette.Tertiary[30];
            scheme.OnTertiaryContainer = corePalette.Tertiary[90];
            scheme.Error = corePalette.Error[80];
            scheme.OnError = corePalette.Error[20];
            scheme.ErrorContainer = corePalette.Error[30];
            scheme.OnErrorContainer = corePalette.Error[80];
            scheme.Background = corePalette.Neutral[10];
            scheme.OnBackground = corePalette.Neutral[90];
            scheme.Surface = corePalette.Neutral[10];
            scheme.OnSurface = corePalette.Neutral[90];
            scheme.SurfaceVariant = corePalette.NeutralVariant[30];
            scheme.OnSurfaceVariant = corePalette.NeutralVariant[80];
            scheme.Outline = corePalette.NeutralVariant[60];
            scheme.Shadow = corePalette.Neutral[0];
            scheme.InverseSurface = corePalette.Neutral[90];
            scheme.InverseOnSurface = corePalette.Neutral[20];
            scheme.InversePrimary = corePalette.Primary[40];
            scheme.Surface1 = scheme.Surface.Add(scheme.Primary, .05);
            scheme.Surface2 = scheme.Surface.Add(scheme.Primary, .08);
            scheme.Surface3 = scheme.Surface.Add(scheme.Primary, .11);
            scheme.Surface4 = scheme.Surface.Add(scheme.Primary, .12);
            scheme.Surface5 = scheme.Surface.Add(scheme.Primary, .14);
        }
    }
}
