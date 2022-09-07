using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MaterialColorUtilities.Maui;

//States:
// Disabled
// Fallback seed
// Custom seed
// Dynamic seed
public class DynamicColorService : DynamicColorService<CorePalette, Scheme<int>, Scheme<Color>, LightSchemeMapper, DarkSchemeMapper>
{
    public DynamicColorService(IOptions<DynamicColorOptions> options, IAccentColorService accentColorService, IApplication application, IPreferences preferences) : base(options, accentColorService, application, preferences)
    {
    }
}

public class DynamicColorService<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    TCorePalette,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
    TSchemeInt,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    TSchemeMaui,
    TLightSchemeMapper,
    TDarkSchemeMapper>
    : IMauiInitializeService
    where TCorePalette : CorePalette
    where TSchemeInt : Scheme<int>, new()
    where TSchemeMaui : Scheme<Color>, new()
    where TLightSchemeMapper : ISchemeMapper<TCorePalette, TSchemeInt>, new()
    where TDarkSchemeMapper : ISchemeMapper<TCorePalette, TSchemeInt>, new()
{
    private const string SeedKey = "MaterialColorUtilities.Maui.Seed";
    private const string IsDarkKey = "MaterialColorUtilities.Maui.IsDark";

    private readonly IAccentColorService _accentColorService;
    private readonly Application _application;
    private readonly ResourceDictionary _appResources;
    private readonly IPreferences _preferences;
    private readonly bool _rememberIsDark;
    private readonly int _fallbackSeed;
    
    private readonly TLightSchemeMapper _lightSchemeMapper = new();
    private readonly TDarkSchemeMapper _darkSchemeMapper = new();

    private bool _enableTheming;
    private bool _enableDynamicColor;
    private int _seed;
    private int? _prevSeed;
    private bool? _prevIsDark;

    public DynamicColorService(
        IOptions<DynamicColorOptions> options,
        IAccentColorService accentColorService,
        IApplication application,
        IPreferences preferences)
    {
        _rememberIsDark = options.Value.RememberIsDark;
        _enableTheming = options.Value.EnableTheming;
        _enableDynamicColor = options.Value.EnableDynamicColor;
        _fallbackSeed = options.Value.FallbackSeed;
        
        _accentColorService = accentColorService;
        _preferences = preferences;
        _application = (Application)application;
        _appResources = _application.Resources;
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
                    ? _preferences.Get(SeedKey, 0)
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
    public int Seed
    {
        get => _seed;
        set
        {
            if (value == _seed) return;
            _seed = value;
            _preferences.Set(SeedKey, value);
            Update();
        }
    }

    public TCorePalette CorePalette { get; protected set; }
    public TSchemeInt SchemeInt { get; protected set; }
    public TSchemeMaui SchemeMaui { get; protected set; }
    
    /// <summary>
    /// When the seed is set, it is stored using Preferences and will be reapplied the next time the app is launched.
    /// Use this to clear the preference and use the fallback seed instead.
    /// </summary>
    public void ForgetSeed()
    {
        Seed = _fallbackSeed;
        _preferences.Clear(SeedKey);
    }

    // Called by MauiAppBuilder.Build()
    public virtual void Initialize(IServiceProvider services)
    {
        if (_preferences.ContainsKey(IsDarkKey))
            _application.UserAppTheme = _preferences.Get(IsDarkKey, false)
                ? AppTheme.Dark
                : AppTheme.Light;

        _seed = _fallbackSeed;
        
        if (_preferences.ContainsKey(SeedKey))
            _seed = _preferences.Get(SeedKey, 0);
        
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
        _accentColorService.OnAccentColorChanged -= Update;
        if (_enableTheming && _enableDynamicColor)
            _accentColorService.OnAccentColorChanged += Update;
        Update();
    }
    
    private void Update()
    {
        if (!EnableTheming) return;

        if (_enableDynamicColor && _accentColorService.AccentColor != null)
            _seed = (int)_accentColorService.AccentColor;
        
        if (Seed != _prevSeed)
            CorePalette = CreateCorePalette(Seed);

        if (Seed == _prevSeed && IsDark == _prevIsDark) return;
        _prevSeed = Seed;
        _prevIsDark = IsDark;
        
        ISchemeMapper<TCorePalette, TSchemeInt> mapper = IsDark
            ? _darkSchemeMapper
            : _lightSchemeMapper;

        SchemeInt = mapper.Map(CorePalette);

        if (typeof(TSchemeMaui) == typeof(Scheme<Color>))
        {
            SchemeMaui = (TSchemeMaui)SchemeInt.ConvertTo(Color.FromInt);
        }
        else
        {
            // We have to use reflection to access the generated method with the correct return type.
            SchemeMaui = (TSchemeMaui)typeof(TSchemeInt)
                .GetMethods()
                .Where(m => m.Name == nameof(Scheme<int>.ConvertTo))
                .ToList()[0]
                .MakeGenericMethod(typeof(Color))
                .Invoke(SchemeInt, new object[] { (Func<int, Color>)Color.FromInt });
        }

#if PLATFORM
        MainThread.BeginInvokeOnMainThread(Apply);
#else
        Apply();
#endif
    }
    
    protected virtual void Apply()
    {
        foreach (PropertyInfo property in typeof(TSchemeMaui).GetProperties())
        {
            string key = property.Name;
            Color value = (Color)property.GetValue(SchemeMaui);
            _appResources[key] = value;
            _appResources[key + "Brush"] = new SolidColorBrush(value);
        }
    }

    /// <summary>Constructs a <typeparamref name="TCorePalette"/>.</summary>
    /// <remarks>
    /// C# doesn't support constructor with parameters as a generic constraint,
    /// so reflection is required to access the constructor. <see href="https://github.com/dotnet/csharplang/discussions/769">Discussion here.</see>
    /// If you replace CorePalette, make sure it has a constructor with the following parameters: <c>int seed, bool isContent</c>
    /// </remarks>
    // TODO: Replace with using empty constructor and method call
    private static TCorePalette CreateCorePalette(int seed)
    {
        return (TCorePalette)Activator.CreateInstance(typeof(TCorePalette), seed, false);
    }
}
