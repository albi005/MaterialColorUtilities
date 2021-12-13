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

    public Color Primary => (IsDark ? GetPrimaryDark() : GetPrimaryLight()).ToColor();
    public Color OnPrimary => (IsDark ? GetOnPrimaryDark() : GetOnPrimaryLight()).ToColor();
    public Color PrimaryContainer => (IsDark ? GetPrimaryContainerDark() : GetPrimaryContainerLight()).ToColor();
    public Color OnPrimaryContainer => (IsDark ? GetOnPrimaryContainerDark() : GetOnPrimaryContainerLight()).ToColor();

    public Color Secondary => (IsDark ? GetSecondaryDark() : GetSecondaryLight()).ToColor();
    public Color OnSecondary => (IsDark ? GetOnSecondaryDark() : GetOnSecondaryLight()).ToColor();
    public Color SecondaryContainer => (IsDark ? GetSecondaryContainerDark() : GetSecondaryContainerLight()).ToColor();
    public Color OnSecondaryContainer => (IsDark ? GetOnSecondaryContainerDark() : GetOnSecondaryContainerLight()).ToColor();

    public Color Tertiary => (IsDark ? GetTertiaryDark() : GetTertiaryLight()).ToColor();
    public Color OnTertiary => (IsDark ? GetOnTertiaryDark() : GetOnTertiaryLight()).ToColor();
    public Color TertiaryContainer => (IsDark ? GetTertiaryContainerDark() : GetTertiaryContainerLight()).ToColor();
    public Color OnTertiaryContainer => (IsDark ? GetOnTertiaryContainerDark() : GetOnTertiaryContainerLight()).ToColor();

    public Color Error => (IsDark ? GetErrorDark() : GetErrorLight()).ToColor();
    public Color OnError => (IsDark ? GetOnErrorDark() : GetOnErrorLight()).ToColor();
    public Color ErrorContainer => (IsDark ? GetErrorContainerDark() : GetErrorContainerLight()).ToColor();
    public Color OnErrorContainer => (IsDark ? GetOnErrorContainerDark() : GetOnErrorContainerLight()).ToColor();

    public Color Background => (IsDark ? GetBackgroundDark() : GetBackgroundLight()).ToColor();
    public Color OnBackground => (IsDark ? GetOnBackgroundDark() : GetOnBackgroundLight()).ToColor();
    public Color Surface => (IsDark ? GetSurfaceDark() : GetSurfaceLight()).ToColor();
    public Color OnSurface => (IsDark ? GetOnSurfaceDark() : GetOnSurfaceLight()).ToColor();
    public Color SurfaceVariant => (IsDark ? GetSurfaceVariantDark() : GetSurfaceVariantLight()).ToColor();
    public Color OnSurfaceVariant => (IsDark ? GetOnSurfaceVariantDark() : GetOnSurfaceVariantLight()).ToColor();
    public Color InverseSurface => (IsDark ? GetInverseOnSurfaceDark() : GetInverseOnSurfaceLight()).ToColor();
    public Color InverseOnSurface => (IsDark ? GetInverseOnSurfaceDark() : GetInverseOnSurfaceLight()).ToColor();
    public Color Outline => (IsDark ? GetOutlineDark() : GetOutlineLight()).ToColor();

    protected virtual uint GetPrimaryLight() => Palette.Primary[40];
    protected virtual uint GetOnPrimaryLight() => Palette.Primary[100];
    protected virtual uint GetPrimaryContainerLight() => Palette.Primary[90];
    protected virtual uint GetOnPrimaryContainerLight() => Palette.Primary[10];
    protected virtual uint GetSecondaryLight() => Palette.Secondary[40];
    protected virtual uint GetOnSecondaryLight() => Palette.Secondary[100];
    protected virtual uint GetSecondaryContainerLight() => Palette.Secondary[90];
    protected virtual uint GetOnSecondaryContainerLight() => Palette.Secondary[10];
    protected virtual uint GetTertiaryLight() => Palette.Tertiary[40];
    protected virtual uint GetOnTertiaryLight() => Palette.Tertiary[100];
    protected virtual uint GetTertiaryContainerLight() => Palette.Tertiary[90];
    protected virtual uint GetOnTertiaryContainerLight() => Palette.Tertiary[10];
    protected virtual uint GetErrorLight() => Palette.Error[40];
    protected virtual uint GetOnErrorLight() => Palette.Error[100];
    protected virtual uint GetErrorContainerLight() => Palette.Error[90];
    protected virtual uint GetOnErrorContainerLight() => Palette.Error[10];
    protected virtual uint GetBackgroundLight() => Palette.Neutral[99];
    protected virtual uint GetOnBackgroundLight() => Palette.Neutral[10];
    protected virtual uint GetSurfaceLight() => Palette.Neutral[99];
    protected virtual uint GetOnSurfaceLight() => Palette.Neutral[10];
    protected virtual uint GetSurfaceVariantLight() => Palette.NeutralVariant[90];
    protected virtual uint GetOnSurfaceVariantLight() => Palette.NeutralVariant[30];
    protected virtual uint GetInverseSurfaceLight() => Palette.Neutral[20];
    protected virtual uint GetInverseOnSurfaceLight() => Palette.Neutral[95];
    protected virtual uint GetOutlineLight() => Palette.NeutralVariant[50];
    protected virtual uint GetPrimaryDark() => Palette.Primary[80];
    protected virtual uint GetOnPrimaryDark() => Palette.Primary[20];
    protected virtual uint GetPrimaryContainerDark() => Palette.Primary[30];
    protected virtual uint GetOnPrimaryContainerDark() => Palette.Primary[90];
    protected virtual uint GetSecondaryDark() => Palette.Secondary[80];
    protected virtual uint GetOnSecondaryDark() => Palette.Secondary[20];
    protected virtual uint GetSecondaryContainerDark() => Palette.Secondary[30];
    protected virtual uint GetOnSecondaryContainerDark() => Palette.Secondary[90];
    protected virtual uint GetTertiaryDark() => Palette.Tertiary[80];
    protected virtual uint GetOnTertiaryDark() => Palette.Tertiary[20];
    protected virtual uint GetTertiaryContainerDark() => Palette.Tertiary[30];
    protected virtual uint GetOnTertiaryContainerDark() => Palette.Tertiary[90];
    protected virtual uint GetErrorDark() => Palette.Error[80];
    protected virtual uint GetOnErrorDark() => Palette.Error[20];
    protected virtual uint GetErrorContainerDark() => Palette.Error[30];
    protected virtual uint GetOnErrorContainerDark() => Palette.Error[90];
    protected virtual uint GetBackgroundDark() => Palette.Neutral[10];
    protected virtual uint GetOnBackgroundDark() => Palette.Neutral[90];
    protected virtual uint GetSurfaceDark() => Palette.Neutral[10];
    protected virtual uint GetOnSurfaceDark() => Palette.Neutral[90];
    protected virtual uint GetSurfaceVariantDark() => Palette.NeutralVariant[30];
    protected virtual uint GetOnSurfaceVariantDark() => Palette.NeutralVariant[80];
    protected virtual uint GetInverseSurfaceDark() => Palette.Neutral[90];
    protected virtual uint GetInverseOnSurfaceDark() => Palette.Neutral[20];
    protected virtual uint GetOutlineDark() => Palette.NeutralVariant[60];
}
