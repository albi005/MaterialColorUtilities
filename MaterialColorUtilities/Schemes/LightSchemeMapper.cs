using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Schemes
{
    /// <inheritdoc/>
    public class LightSchemeMapper : LightSchemeMapper<CorePalette, Scheme<uint>>
    {
    }

    /// <summary>
    /// A mapper that maps a core palette to a light scheme using the 
    /// <see href="https://m3.material.io/styles/color/the-color-system/tokens">default Material Design 3 mappings.</see>
    /// </summary>
    public class LightSchemeMapper<TCorePalette, TScheme> : BaseSchemeMapper<TCorePalette, TScheme>
        where TCorePalette : CorePalette
        where TScheme : Scheme<uint>, new()
    {
        protected override void MapCore(TCorePalette corePalette, TScheme scheme)
        {
            scheme.Primary = corePalette.Primary[40];
            scheme.OnPrimary = corePalette.Primary[100];
            scheme.PrimaryContainer = corePalette.Primary[90];
            scheme.OnPrimaryContainer = corePalette.Primary[10];
            scheme.Secondary = corePalette.Secondary[40];
            scheme.OnSecondary = corePalette.Secondary[100];
            scheme.SecondaryContainer = corePalette.Secondary[90];
            scheme.OnSecondaryContainer = corePalette.Secondary[10];
            scheme.Tertiary = corePalette.Tertiary[40];
            scheme.OnTertiary = corePalette.Tertiary[100];
            scheme.TertiaryContainer = corePalette.Tertiary[90];
            scheme.OnTertiaryContainer = corePalette.Tertiary[10];
            scheme.Error = corePalette.Error[40];
            scheme.OnError = corePalette.Error[100];
            scheme.ErrorContainer = corePalette.Error[90];
            scheme.OnErrorContainer = corePalette.Error[10];
            scheme.Background = corePalette.Neutral[99];
            scheme.OnBackground = corePalette.Neutral[10];
            scheme.Surface = corePalette.Neutral[99];
            scheme.OnSurface = corePalette.Neutral[10];
            scheme.SurfaceVariant = corePalette.NeutralVariant[90];
            scheme.OnSurfaceVariant = corePalette.NeutralVariant[30];
            scheme.Outline = corePalette.NeutralVariant[50];
            scheme.Shadow = corePalette.Neutral[0];
            scheme.InverseSurface = corePalette.Neutral[20];
            scheme.InverseOnSurface = corePalette.Neutral[95];
            scheme.InversePrimary = corePalette.Primary[80];
            scheme.Surface1 = scheme.Surface.Add(scheme.Primary, .05);
            scheme.Surface2 = scheme.Surface.Add(scheme.Primary, .08);
            scheme.Surface3 = scheme.Surface.Add(scheme.Primary, .11);
            scheme.Surface4 = scheme.Surface.Add(scheme.Primary, .12);
            scheme.Surface5 = scheme.Surface.Add(scheme.Primary, .14);
        }
    }
}
