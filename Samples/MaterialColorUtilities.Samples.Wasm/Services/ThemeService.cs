using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Samples.Wasm.Extensions;
using MudBlazor;
using MudBlazor.Utilities;

namespace MaterialColorUtilities.Samples.Wasm.Services;

public class ThemeService
{
    private int _seed = Score.Scorer.Default;

    public ThemeService() => Apply();

    public int Seed
    {
        get => _seed;
        set
        {
            _seed = value;
            Apply();
        }
    }

    public void Apply()
    {
        CorePalette corePalette = new(Seed);
        AppScheme<MudColor> lightScheme = new LightAppSchemeMapper()
            .Map(corePalette)
            .ConvertTo(IntExtensions.ToMudColor);
        MudTheme.Palette = new()
        {
            Primary = lightScheme.Primary,
            Secondary = lightScheme.Secondary,
            Tertiary = lightScheme.Tertiary,
            Background = lightScheme.Background,
            AppbarBackground = lightScheme.Elevation2,
            AppbarText = lightScheme.OnBackground,
            DrawerBackground = lightScheme.Elevation1,
            Surface = lightScheme.Elevation1,
        };
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public MudTheme MudTheme { get; } = new()
    {
        ZIndex = new()
        {
            AppBar = 2000
        }
    };
    public event EventHandler ThemeChanged;
}
