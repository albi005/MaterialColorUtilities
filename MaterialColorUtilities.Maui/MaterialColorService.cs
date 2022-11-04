using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using Style = MaterialColorUtilities.Palettes.Style;

namespace MaterialColorUtilities.Maui;

//States:
// Disabled
// Fallback seed
// Custom seed
// Dynamic seed
public class MaterialColorService : MaterialColorService<CorePalette, Scheme<uint>, Scheme<Color>, LightSchemeMapper, DarkSchemeMapper>
{
    public MaterialColorService(IOptions<MaterialColorOptions> options, IDynamicColorService dynamicColorService, IPreferences preferences) : base(options, dynamicColorService, preferences)
    {
    }
}

public class MaterialColorService<
    TCorePalette,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
    TSchemeInt,
    TSchemeMaui,
    TLightSchemeMapper,
    TDarkSchemeMapper>
    : IMaterialColorService
    where TCorePalette : CorePalette, new()
    where TSchemeInt : Scheme<uint>, new()
    where TSchemeMaui : Scheme<Color>, new()
    where TLightSchemeMapper : ISchemeMapper<TCorePalette, TSchemeInt>, new()
    where TDarkSchemeMapper : ISchemeMapper<TCorePalette, TSchemeInt>, new()
{
    private const string SeedKey = "MaterialColorUtilities.Maui.Seed";
    private const string IsDarkKey = "MaterialColorUtilities.Maui.IsDark";
    private const string StyleKey = "MaterialColorUtilities.Maui.Style";

    private readonly IDynamicColorService _dynamicColorService;
    private readonly IPreferences _preferences;
    private readonly bool _rememberIsDark;
    private readonly uint _fallbackSeed;
    
    private readonly TLightSchemeMapper _lightSchemeMapper = new();
    private readonly TDarkSchemeMapper _darkSchemeMapper = new();

    private Application _application = null!;
    private ResourceDictionary _appResources = null!;

    private bool _enableTheming;
    private bool _enableDynamicColor;
    private uint _seed;
    private Style _style;
    private uint? _prevSeed;
    private bool? _prevIsDark;
    private Style _prevStyle;

    public MaterialColorService(
        IOptions<MaterialColorOptions> options,
        IDynamicColorService dynamicColorService,
        IPreferences preferences)
    {
        _rememberIsDark = options.Value.RememberIsDark;
        _enableTheming = options.Value.EnableTheming;
        _enableDynamicColor = options.Value.EnableDynamicColor;
        _fallbackSeed = options.Value.FallbackSeed;
        _style = options.Value.DefaultStyle;
        
        _dynamicColorService = dynamicColorService;
        _preferences = preferences;

        IMaterialColorService.Current = this;
    }
    
    public bool EnableTheming
    {
        get => _enableTheming;
        set
        {
            if (value == _enableTheming) return;
            _enableTheming = value;
            
            OnOptionsChanged();
        }
    }

    public bool EnableDynamicColor
    {
        get => _enableDynamicColor;
        set
        {
            if (value == _enableDynamicColor) return;
            _enableDynamicColor = value;

            if (!value)
            {
                _seed = _preferences.ContainsKey(SeedKey)
                    ? (uint)_preferences.Get(SeedKey, 0)
                    : _fallbackSeed;
            }

            OnOptionsChanged();
        }
    }

    /// <summary>
    /// Decides if a dark scheme should be generated instead of light.
    /// </summary>
    /// <remarks>
    /// To update, use <see cref="Application.UserAppTheme"/>.
    /// </remarks>
    public bool IsDark => _application.RequestedTheme == AppTheme.Dark;

    /// <summary>
    /// A color in ARGB format, that is used as seed when creating the color scheme.
    /// </summary>
    /// <remarks>
    /// Changes will be saved using Preferences and reapplied the next time the app is launched.
    /// </remarks>
    public uint Seed
    {
        get => _seed;
        set
        {
            if (value == _seed) return;
            _seed = value;
            _preferences.Set(SeedKey, (int)value);
            Update();
        }
    }
    
    /// <summary>
    /// The style used to create the core palette.
    /// </summary>
    /// <remarks>
    /// Changes will be saved using Preferences and reapplied the next time the app is launched.
    /// </remarks>
    public Style Style
    {
        get => _style;
        set
        {
            if (value == _style) return;
            _style = value;
            _preferences.Set(StyleKey, (int)value);
            Update();
        }
    }

    public TCorePalette CorePalette { get; } = new();
    public TSchemeInt SchemeInt { get; protected set; } = null!;
    public TSchemeMaui SchemeMaui { get; protected set; } = null!;
    
    /// <summary>
    /// When the seed is set, it is stored using Preferences and will be reapplied the next time the app is launched.
    /// Use this to clear the preference and use the fallback seed instead.
    /// </summary>
    public void ForgetSeed()
    {
        Seed = _fallbackSeed;
        _preferences.Clear(SeedKey);
    }

    // Called automatically by MauiAppBuilder.Build()
    void IMauiInitializeService.Initialize(IServiceProvider? serviceProvider) { }

    public virtual void Initialize(ResourceDictionary resourceDictionary)
    {
        _application = Application.Current!;
        _appResources = resourceDictionary;
        
        if (_preferences.ContainsKey(IsDarkKey))
            _application.UserAppTheme = _preferences.Get(IsDarkKey, false)
                ? AppTheme.Dark
                : AppTheme.Light;

        _seed = _fallbackSeed;
        
        if (_preferences.ContainsKey(SeedKey))
            _seed = (uint)_preferences.Get(SeedKey, 0);

        if (_preferences.ContainsKey(StyleKey))
            _style = (Style)_preferences.Get(StyleKey, (int)Style.TonalSpot);
        
        _application.RequestedThemeChanged += (_, _) =>
        {
            if (_rememberIsDark)
            {
                if (_application.UserAppTheme == AppTheme.Unspecified)
                    _preferences.Remove(IsDarkKey);
                else
                    _preferences.Set(IsDarkKey, _application.UserAppTheme == AppTheme.Dark);
            }
            Update();
        };

        OnOptionsChanged();
    }

    private void OnOptionsChanged()
    {
        _dynamicColorService.Changed -= Update;
        if (_enableTheming && _enableDynamicColor)
            _dynamicColorService.Changed += Update;
        Update();
    }
    
    private void Update()
    {
        if (!EnableTheming) return;

        if (_enableDynamicColor && _dynamicColorService.SeedColor != null)
            _seed = (uint)_dynamicColorService.SeedColor;

        if (Seed != _prevSeed || Style != _prevStyle)
            CorePalette.Fill(Seed, Style);

        if (_enableDynamicColor && _dynamicColorService.CorePalette != null)
        {
            CorePalette.Primary = _dynamicColorService.CorePalette.Primary;
            CorePalette.Secondary = _dynamicColorService.CorePalette.Secondary;
            CorePalette.Tertiary = _dynamicColorService.CorePalette.Tertiary;
            CorePalette.Neutral = _dynamicColorService.CorePalette.Neutral;
            CorePalette.NeutralVariant = _dynamicColorService.CorePalette.NeutralVariant;
        }

        if (Seed == _prevSeed && IsDark == _prevIsDark && Style == _prevStyle) return;
        _prevSeed = Seed;
        _prevIsDark = IsDark;
        _prevStyle = Style;
        
        ISchemeMapper<TCorePalette, TSchemeInt> mapper = IsDark
            ? _darkSchemeMapper
            : _lightSchemeMapper;

        SchemeInt = mapper.Map(CorePalette);

        if (typeof(TSchemeMaui) == typeof(Scheme<Color>))
        {
            SchemeMaui = (TSchemeMaui)SchemeInt.Convert(Color.FromUint);
        }
        else
        {
            // We have to use reflection to access the generated method with the correct return type.
            SchemeMaui = (TSchemeMaui)typeof(TSchemeInt)
                .GetMethods()
                .Where(m => m.Name == nameof(Scheme<int>.Convert))
                .ToList()[0]
                .MakeGenericMethod(typeof(Color))
                .Invoke(SchemeInt, new object[] { (Func<uint, Color>)Color.FromUint })!;
        }

#if PLATFORM
        MainThread.BeginInvokeOnMainThread(Apply);
#else
        Apply();
#endif
    }
    
    protected virtual void Apply()
    {
        foreach (KeyValuePair<string, Color> color in SchemeMaui.Enumerate())
        {
            _appResources[color.Key] = color.Value;
            _appResources[color.Key + "Brush"] = new SolidColorBrush(color.Value);
        }
    }
}

public interface IMaterialColorService : IMauiInitializeService
{
    public static IMaterialColorService Current { get; set; } = null!;

    void Initialize(ResourceDictionary resourceDictionary);
}