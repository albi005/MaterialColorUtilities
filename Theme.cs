using Monet.Extensions;
using System.Drawing;

namespace Monet;

public class Theme
{
    public const string PrimaryKey = nameof(Primary);
    public const string PrimaryContainerKey = nameof(PrimaryContainer);
    public const string SecondaryKey = nameof(Secondary);
    public const string SecondaryContainerKey = nameof(SecondaryContainer);
    public const string TertiaryKey = nameof(Tertiary);
    public const string TertiaryContainerKey = nameof(TertiaryContainer);
    public const string SurfaceKey = nameof(Surface);
    public const string SurfaceVariantKey = nameof(SurfaceVariant);
    public const string BackgroundKey = nameof(Background);
    public const string ErrorKey = nameof(Error);
    public const string ErrorContainerKey = nameof(ErrorContainer);
    public const string OnPrimaryKey = nameof(OnPrimary);
    public const string OnPrimaryContainerKey = nameof(OnPrimaryContainer);
    public const string OnSecondaryKey = nameof(OnSecondary);
    public const string OnSecondaryContainerKey = nameof(OnSecondaryContainer);
    public const string OnTertiaryKey = nameof(OnTertiary);
    public const string OnTertiaryContainerKey = nameof(OnTertiaryContainer);
    public const string OnSurfaceKey = nameof(OnSurface);
    public const string OnSurfaceVariantKey = nameof(OnSurfaceVariant);
    public const string OnErrorKey = nameof(OnError);
    public const string OnErrorContainerKey = nameof(OnErrorContainer);
    public const string OnBackgroundKey = nameof(OnBackground);
    public const string OutlineKey = nameof(Outline);
    public const string InverseSurfaceKey = nameof(InverseSurface);
    public const string InverseOnSurfaceKey = nameof(InverseOnSurface);

    public CorePalette Palette { get; set; }
    public bool IsDark { get; set; } = false;

    public Theme(CorePalette palette) => Palette = palette;

    public Color Primary => IsDark ? PrimaryDark : PrimaryLight;
    public Color OnPrimary => IsDark ? OnPrimaryDark : OnPrimaryLight;
    public Color PrimaryContainer => IsDark ? PrimaryContainerDark : PrimaryContainerLight;
    public Color OnPrimaryContainer => IsDark ? OnPrimaryContainerDark : OnPrimaryContainerLight;

    public Color Secondary => IsDark ? SecondaryDark : SecondaryLight;
    public Color OnSecondary => IsDark ? OnSecondaryDark : OnSecondaryLight;
    public Color SecondaryContainer => IsDark ? SecondaryContainerDark : SecondaryContainerLight;
    public Color OnSecondaryContainer => IsDark ? OnSecondaryContainerDark : OnSecondaryContainerLight;

    public Color Tertiary => IsDark ? TertiaryDark : TertiaryLight;
    public Color OnTertiary => IsDark ? OnTertiaryDark : OnTertiaryLight;
    public Color TertiaryContainer => IsDark ? TertiaryContainerDark : TertiaryContainerLight;
    public Color OnTertiaryContainer => IsDark ? OnTertiaryContainerDark : OnTertiaryContainerLight;

    public Color Error => IsDark ? ErrorDark : ErrorLight;
    public Color OnError => IsDark ? OnErrorDark : OnErrorLight;
    public Color ErrorContainer => IsDark ? ErrorContainerDark : ErrorContainerLight;
    public Color OnErrorContainer => IsDark ? OnErrorContainerDark : OnErrorContainerLight;

    public Color Background => IsDark ? BackgroundDark : BackgroundLight;
    public Color OnBackground => IsDark ? OnBackgroundDark : OnBackgroundLight;
    public Color Surface => IsDark ? SurfaceDark : SurfaceLight;
    public Color OnSurface => IsDark ? OnSurfaceDark : OnSurfaceLight;
    public Color SurfaceVariant => IsDark ? SurfaceVariantDark : SurfaceVariantLight;
    public Color OnSurfaceVariant => IsDark ? OnSurfaceVariantDark : OnSurfaceVariantLight;
    public Color InverseSurface => IsDark ? InverseOnSurfaceDark : InverseOnSurfaceLight;
    public Color InverseOnSurface => IsDark ? InverseOnSurfaceDark : InverseOnSurfaceLight;
    public Color Outline => IsDark ? OutlineDark : OutlineLight;

    // LIGHT
    public Color PrimaryLight => Palette.Primary[40].ToColor();
    public Color OnPrimaryLight => Palette.Primary[100].ToColor();
    public Color PrimaryContainerLight => Palette.Primary[90].ToColor();
    public Color OnPrimaryContainerLight => Palette.Primary[10].ToColor();

    public Color SecondaryLight => Palette.Secondary[40].ToColor();
    public Color OnSecondaryLight => Palette.Secondary[100].ToColor();
    public Color SecondaryContainerLight => Palette.Secondary[90].ToColor();
    public Color OnSecondaryContainerLight => Palette.Secondary[10].ToColor();

    public Color TertiaryLight => Palette.Tertiary[40].ToColor();
    public Color OnTertiaryLight => Palette.Tertiary[100].ToColor();
    public Color TertiaryContainerLight => Palette.Tertiary[90].ToColor();
    public Color OnTertiaryContainerLight => Palette.Tertiary[10].ToColor();

    public Color ErrorLight => Palette.Error[40].ToColor();
    public Color OnErrorLight => Palette.Error[100].ToColor();
    public Color ErrorContainerLight => Palette.Error[90].ToColor();
    public Color OnErrorContainerLight => Palette.Error[10].ToColor();

    public Color BackgroundLight => Palette.Neutral[99].ToColor();
    public Color OnBackgroundLight => Palette.Neutral[10].ToColor();
    public Color SurfaceLight => Palette.Neutral[99].ToColor();
    public Color OnSurfaceLight => Palette.Neutral[10].ToColor();
    public Color SurfaceVariantLight => Palette.NeutralVariant[90].ToColor();
    public Color OnSurfaceVariantLight => Palette.NeutralVariant[30].ToColor();
    public Color InverseSurfaceLight => Palette.Neutral[20].ToColor();
    public Color InverseOnSurfaceLight => Palette.Neutral[95].ToColor();
    public Color OutlineLight => Palette.NeutralVariant[50].ToColor();

    // DARK
    public Color PrimaryDark => Palette.Primary[80].ToColor();
    public Color OnPrimaryDark => Palette.Primary[20].ToColor();
    public Color PrimaryContainerDark => Palette.Primary[30].ToColor();
    public Color OnPrimaryContainerDark => Palette.Primary[90].ToColor();

    public Color SecondaryDark => Palette.Secondary[80].ToColor();
    public Color OnSecondaryDark => Palette.Secondary[20].ToColor();
    public Color SecondaryContainerDark => Palette.Secondary[30].ToColor();
    public Color OnSecondaryContainerDark => Palette.Secondary[90].ToColor();

    public Color TertiaryDark => Palette.Tertiary[80].ToColor();
    public Color OnTertiaryDark => Palette.Tertiary[20].ToColor();
    public Color TertiaryContainerDark => Palette.Tertiary[30].ToColor();
    public Color OnTertiaryContainerDark => Palette.Tertiary[90].ToColor();

    public Color ErrorDark => Palette.Error[80].ToColor();
    public Color OnErrorDark => Palette.Error[20].ToColor();
    public Color ErrorContainerDark => Palette.Error[30].ToColor();
    public Color OnErrorContainerDark => Palette.Error[90].ToColor();

    public Color BackgroundDark => Palette.Neutral[10].ToColor();
    public Color OnBackgroundDark => Palette.Neutral[90].ToColor();
    public Color SurfaceDark => Palette.Neutral[10].ToColor();
    public Color OnSurfaceDark => Palette.Neutral[90].ToColor();
    public Color SurfaceVariantDark => Palette.NeutralVariant[30].ToColor();
    public Color OnSurfaceVariantDark => Palette.NeutralVariant[80].ToColor();
    public Color InverseSurfaceDark => Palette.Neutral[90].ToColor();
    public Color InverseOnSurfaceDark => Palette.Neutral[20].ToColor();
    public Color OutlineDark => Palette.NeutralVariant[60].ToColor();
}
