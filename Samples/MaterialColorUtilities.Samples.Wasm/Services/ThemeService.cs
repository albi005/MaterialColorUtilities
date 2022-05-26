using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Samples.Shared;
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

    public AppScheme<int> Scheme { get; set; }

    public void Apply()
    {
        CorePalette corePalette = new(Seed);
        Scheme = new LightAppSchemeMapper().Map(corePalette);
        AppScheme<MudColor> mudScheme = Scheme.ConvertTo(IntExtensions.ToMudColor);
        MudTheme.Palette = new()
        {
            Primary = mudScheme.Primary,
            Secondary = mudScheme.Secondary,
            Tertiary = mudScheme.Tertiary,
            Background = mudScheme.Background,
            AppbarBackground = mudScheme.Elevation2,
            AppbarText = mudScheme.OnBackground,
            DrawerBackground = mudScheme.Elevation1,
            Surface = mudScheme.Elevation1,
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
