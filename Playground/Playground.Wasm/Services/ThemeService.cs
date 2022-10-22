using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using MaterialColorUtilities.Score;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Utilities;
using Playground.Shared;
using Playground.Wasm.Extensions;

namespace Playground.Wasm.Services;

public class ThemeService
{
    private bool _isDark;
    private readonly LightAppSchemeMapper _lightMapper = new();
    private readonly DarkAppSchemeMapper _darkMapper = new();
    private uint _seed = Scorer.Default;

    private uint? _prevSeed;
    private bool? _prevIsDark;

    public ThemeService()
    {
        Current = this;
        Apply();
    }

    public static ThemeService Current { get; private set; }

    public bool IsDark
    {
        get => _isDark;
        set
        {
            _isDark = value;
            Apply();
        }
    }

    public uint Seed
    {
        get => _seed;
        set { _seed = value; Apply(); }
    }

    private CorePalette CorePalette { get; set; }

    public AppScheme<uint> Scheme { get; private set; }
    public MudTheme MudTheme { get; } = new()
    {
        ZIndex = new()
        {
            AppBar = 2000
        }
    };

    public event Action Changed;

    private void Apply()
    {
        if (_isDark == _prevIsDark && _seed == _prevSeed) return;
        _prevIsDark = _isDark;
        
        if (_seed != _prevSeed)
        {
            CorePalette = new(_seed);
            _prevSeed = _seed;
        }

        ISchemeMapper<CorePalette, AppScheme<uint>> mapper = IsDark
            ? _darkMapper
            : _lightMapper;
        Scheme = mapper.Map(CorePalette);
        AppScheme<MudColor> mudColorScheme = Scheme.ConvertTo(IntExtensions.ToMudColor);
        if (IsDark)
            MudTheme.PaletteDark = UpdatePalette(MudTheme.PaletteDark, mudColorScheme);
        else
            MudTheme.Palette = UpdatePalette(MudTheme.Palette, mudColorScheme);
        Changed?.Invoke();
    }

    private static Palette UpdatePalette(Palette palette, AppScheme<MudColor> scheme)
    {
        palette.Primary = scheme.Primary;
        palette.Secondary = scheme.Secondary;
        palette.Tertiary = scheme.Tertiary;
        palette.Background = scheme.Background;
        palette.AppbarBackground = scheme.Surface2;
        palette.AppbarText = scheme.OnBackground;
        palette.DrawerBackground = scheme.Surface1;
        palette.Surface = scheme.Surface1;
        return palette;
    }

    [JSInvokable]
    public static void OnIsDarkChanged(bool isDark) => Current.IsDark = isDark;
}
