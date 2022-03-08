using MaterialColorUtilities.Palettes;

namespace MaterialColorUtilities.Schemes;

public class LightScheme : Scheme<int>
{
    private readonly CorePalette corePalette;

    public LightScheme(CorePalette corePalette)
    {
        this.corePalette = corePalette;
        Primary = PrimaryLight;
        OnPrimary = OnPrimaryLight;
        PrimaryContainer = PrimaryContainerLight;
        OnPrimaryContainer = OnPrimaryContainerLight;
        Secondary = SecondaryLight;
        OnSecondary = OnSecondaryLight;
        SecondaryContainer = SecondaryContainerLight;
        OnSecondaryContainer = OnSecondaryContainerLight;
        Tertiary = TertiaryLight;
        OnTertiary = OnTertiaryLight;
        TertiaryContainer = TertiaryContainerLight;
        OnTertiaryContainer = OnTertiaryContainerLight;
        Error = ErrorLight;
        OnError = OnErrorLight;
        ErrorContainer = ErrorContainerLight;
        OnErrorContainer = OnErrorContainerLight;
        Background = BackgroundLight;
        OnBackground = OnBackgroundLight;
        Surface = SurfaceLight;
        OnSurface = OnSurfaceLight;
        SurfaceVariant = SurfaceVariantLight;
        OnSurfaceVariant = OnSurfaceVariantLight;
        Outline = OutlineLight;
        Shadow = ShadowLight;
        InverseSurface = InverseSurfaceLight;
        InverseOnSurface = InverseOnSurfaceLight;
        InversePrimary = InversePrimaryLight;
    }

    protected virtual int PrimaryLight => corePalette.Primary[40];
    protected virtual int OnPrimaryLight => corePalette.Primary[100];
    protected virtual int PrimaryContainerLight => corePalette.Primary[90];
    protected virtual int OnPrimaryContainerLight => corePalette.Primary[10];
    protected virtual int SecondaryLight => corePalette.Secondary[40];
    protected virtual int OnSecondaryLight => corePalette.Secondary[100];
    protected virtual int SecondaryContainerLight => corePalette.Secondary[90];
    protected virtual int OnSecondaryContainerLight => corePalette.Secondary[10];
    protected virtual int TertiaryLight => corePalette.Tertiary[40];
    protected virtual int OnTertiaryLight => corePalette.Tertiary[100];
    protected virtual int TertiaryContainerLight => corePalette.Tertiary[90];
    protected virtual int OnTertiaryContainerLight => corePalette.Tertiary[10];
    protected virtual int ErrorLight => corePalette.Error[40];
    protected virtual int OnErrorLight => corePalette.Error[100];
    protected virtual int ErrorContainerLight => corePalette.Error[90];
    protected virtual int OnErrorContainerLight => corePalette.Error[10];
    protected virtual int BackgroundLight => corePalette.Neutral[99];
    protected virtual int OnBackgroundLight => corePalette.Neutral[10];
    protected virtual int SurfaceLight => corePalette.Neutral[99];
    protected virtual int OnSurfaceLight => corePalette.Neutral[10];
    protected virtual int SurfaceVariantLight => corePalette.NeutralVariant[90];
    protected virtual int OnSurfaceVariantLight => corePalette.NeutralVariant[30];
    protected virtual int OutlineLight => corePalette.NeutralVariant[50];
    protected virtual int ShadowLight => corePalette.Neutral[0];
    protected virtual int InverseSurfaceLight => corePalette.Neutral[20];
    protected virtual int InverseOnSurfaceLight => corePalette.Neutral[95];
    protected virtual int InversePrimaryLight => corePalette.Primary[80];
}
