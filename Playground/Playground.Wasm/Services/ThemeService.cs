using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using MaterialColorUtilities.Score;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Utilities;
using Playground.Wasm.Extensions;

namespace Playground.Wasm.Services;

public class ThemeService
{
    private bool _isDark;
    private readonly LightMudSchemeMapper _lightMapper = new();
    private readonly DarkMudSchemeMapper _darkMapper = new();
    private uint _seed = Scorer.Default;
    private Style _style = Style.Vibrant;

    private uint? _prevSeed;
    private bool? _prevIsDark;
    private Style? _prevStyle;

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

    public Style Style
    {
        get => _style;
        set
        {
            _style = value; 
            Apply();
        }
    }

    private CorePalette CorePalette { get; } = new();

    public MudScheme<uint> Scheme { get; private set; }
    public MudScheme<uint> LightScheme { get; private set; }
    public MudScheme<uint> DarkScheme { get; private set; }

    public MudTheme MudTheme { get; } = new()
    {
        ZIndex = new()
        {
            AppBar = 2000
        }
    };

    public event Action Changed;
    
    public string CreateCssVariables() => CssVariables.Create(LightScheme, DarkScheme);

    private void Apply()
    {
        if (_isDark == _prevIsDark && _seed == _prevSeed && _style == _prevStyle) return;
        _prevIsDark = _isDark;

        if (_seed != _prevSeed || _style != _prevStyle)
        {
            _prevSeed = _seed;
            _prevStyle = _style;
            CorePalette.Fill(_seed, _style);
        }
        
        LightScheme = _lightMapper.Map(CorePalette);
        DarkScheme = _darkMapper.Map(CorePalette);
        Scheme = IsDark
            ? DarkScheme
            : LightScheme;

        MudScheme<MudColor> mudColorScheme = Scheme.Convert(IntExtensions.ToMudColor);
        if (IsDark)
            MudTheme.PaletteDark = UpdatePalette(MudTheme.PaletteDark, mudColorScheme);
        else
            MudTheme.Palette = UpdatePalette(MudTheme.Palette, mudColorScheme);
        Changed?.Invoke();
    }

    private static Palette UpdatePalette(Palette palette, MudScheme<MudColor> scheme)
    {
        palette.Primary = scheme.Primary;
        palette.PrimaryDarken = scheme.Primary2.Value;
        palette.PrimaryContrastText = scheme.OnPrimary;
        palette.Secondary = scheme.Secondary;
        palette.SecondaryContrastText = scheme.OnSecondary;
        palette.Tertiary = scheme.Tertiary;
        palette.TertiaryContrastText = scheme.OnTertiary;
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
