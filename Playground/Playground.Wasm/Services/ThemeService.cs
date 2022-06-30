using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using MaterialColorUtilities.Score;
using MudBlazor;
using MudBlazor.Utilities;
using Playground.Shared;
using Playground.Wasm.Extensions;

namespace Playground.Wasm.Services;

public class ThemeService
{
    private int _seed = Scorer.Default;
    private bool _isDark;
    private readonly LightAppSchemeMapper _lightMapper = new();
    private readonly DarkAppSchemeMapper _darkMapper = new();

    public ThemeService() => Apply();

    public bool IsDark
    {
        get => _isDark;
        set
        {
            _isDark = value;
            Apply();
            ThemeChanged?.Invoke();
        }
    }
    public int Seed => _seed;
    public AppScheme<int> Scheme { get; set; }
    public MudTheme MudTheme { get; } = new()
    {
        ZIndex = new()
        {
            AppBar = 2000
        }
    };

    public event EventHandler<int> SeedChanged;
    public event Action ThemeChanged;

    public void SetSeed(int value, object sender)
    {
        _seed = value;
        SeedChanged?.Invoke(sender, value);
        Apply();
    }

    private void Apply()
    {
        CorePalette corePalette = new(Seed);
        ISchemeMapper<CorePalette, AppScheme<int>> mapper = IsDark
            ? _darkMapper
            : _lightMapper;
        Scheme = mapper.Map(corePalette);
        AppScheme<MudColor> mudColorScheme = Scheme.ConvertTo(IntExtensions.ToMudColor);
        if (IsDark)
            MudTheme.PaletteDark = UpdatePalette(MudTheme.PaletteDark, mudColorScheme);
        else
            MudTheme.Palette = UpdatePalette(MudTheme.Palette, mudColorScheme);
        ThemeChanged?.Invoke();
    }

    private static Palette UpdatePalette(Palette palette, AppScheme<MudColor> scheme)
    {
        palette.Primary = scheme.Primary;
        palette.Secondary = scheme.Secondary;
        palette.Tertiary = scheme.Tertiary;
        palette.Background = scheme.Background;
        palette.AppbarBackground = scheme.Elevation2;
        palette.AppbarText = scheme.OnBackground;
        palette.DrawerBackground = scheme.Elevation1;
        palette.Surface = scheme.Elevation1;
        return palette;
    }
}
