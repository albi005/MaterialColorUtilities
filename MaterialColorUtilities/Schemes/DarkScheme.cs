using MaterialColorUtilities.Palettes;

namespace MaterialColorUtilities.Schemes;

public class DarkScheme : Scheme<int>
{
    private readonly CorePalette corePalette;

    public DarkScheme(CorePalette corePalette)
    {
        this.corePalette = corePalette;
        Primary = PrimaryDark;
        OnPrimary = OnPrimaryDark;
        PrimaryContainer = PrimaryContainerDark;
        OnPrimaryContainer = OnPrimaryContainerDark;
        Secondary = SecondaryDark;
        OnSecondary = OnSecondaryDark;
        SecondaryContainer = SecondaryContainerDark;
        OnSecondaryContainer = OnSecondaryContainerDark;
        Tertiary = TertiaryDark;
        OnTertiary = OnTertiaryDark;
        TertiaryContainer = TertiaryContainerDark;
        OnTertiaryContainer = OnTertiaryContainerDark;
        Error = ErrorDark;
        OnError = OnErrorDark;
        ErrorContainer = ErrorContainerDark;
        OnErrorContainer = OnErrorContainerDark;
        Background = BackgroundDark;
        OnBackground = OnBackgroundDark;
        Surface = SurfaceDark;
        OnSurface = OnSurfaceDark;
        SurfaceVariant = SurfaceVariantDark;
        OnSurfaceVariant = OnSurfaceVariantDark;
        Outline = OutlineDark;
        Shadow = ShadowDark;
        InverseSurface = InverseSurfaceDark;
        InverseOnSurface = InverseOnSurfaceDark;
        InversePrimary = InversePrimaryDark;
    }

    protected virtual int PrimaryDark => corePalette.Primary[80];
    protected virtual int OnPrimaryDark => corePalette.Primary[20];
    protected virtual int PrimaryContainerDark => corePalette.Primary[30];
    protected virtual int OnPrimaryContainerDark => corePalette.Primary[90];
    protected virtual int SecondaryDark => corePalette.Secondary[80];
    protected virtual int OnSecondaryDark => corePalette.Secondary[20];
    protected virtual int SecondaryContainerDark => corePalette.Secondary[30];
    protected virtual int OnSecondaryContainerDark => corePalette.Secondary[90];
    protected virtual int TertiaryDark => corePalette.Tertiary[80];
    protected virtual int OnTertiaryDark => corePalette.Tertiary[20];
    protected virtual int TertiaryContainerDark => corePalette.Tertiary[30];
    protected virtual int OnTertiaryContainerDark => corePalette.Tertiary[90];
    protected virtual int ErrorDark => corePalette.Error[80];
    protected virtual int OnErrorDark => corePalette.Error[20];
    protected virtual int ErrorContainerDark => corePalette.Error[30];
    protected virtual int OnErrorContainerDark => corePalette.Error[90];
    protected virtual int BackgroundDark => corePalette.Neutral[10];
    protected virtual int OnBackgroundDark => corePalette.Neutral[90];
    protected virtual int SurfaceDark => corePalette.Neutral[10];
    protected virtual int OnSurfaceDark => corePalette.Neutral[90];
    protected virtual int SurfaceVariantDark => corePalette.NeutralVariant[30];
    protected virtual int OnSurfaceVariantDark => corePalette.NeutralVariant[80];
    protected virtual int OutlineDark => corePalette.NeutralVariant[60];
    protected virtual int ShadowDark => corePalette.Neutral[0];
    protected virtual int InverseSurfaceDark => corePalette.Neutral[90];
    protected virtual int InverseOnSurfaceDark => corePalette.Neutral[20];
    protected virtual int InversePrimaryDark => corePalette.Primary[40];
}
