﻿using MaterialColorUtilities.Palettes;
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
public class MaterialColorService : MaterialColorService<CorePalette, Scheme<uint>, Scheme<Color>, LightSchemeMapper, DarkSchemeMapper>
{
    public MaterialColorService(IOptions<MaterialColorOptions> options, IDynamicColorService dynamicColorService, IApplication application, IPreferences preferences) : base(options, dynamicColorService, application, preferences)
    {
    }
}

public class MaterialColorService<
    TCorePalette,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
    TSchemeInt,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    TSchemeMaui,
    TLightSchemeMapper,
    TDarkSchemeMapper>
    : IMauiInitializeService
    where TCorePalette : CorePalette, new()
    where TSchemeInt : Scheme<uint>, new()
    where TSchemeMaui : Scheme<Color>, new()
    where TLightSchemeMapper : ISchemeMapper<TCorePalette, TSchemeInt>, new()
    where TDarkSchemeMapper : ISchemeMapper<TCorePalette, TSchemeInt>, new()
{
    private const string SeedKey = "MaterialColorUtilities.Maui.Seed";
    private const string IsDarkKey = "MaterialColorUtilities.Maui.IsDark";

    private readonly IDynamicColorService _dynamicColorService;
    private readonly Application _application;
    private readonly ResourceDictionary _appResources;
    private readonly IPreferences _preferences;
    private readonly bool _rememberIsDark;
    private readonly uint _fallbackSeed;
    
    private readonly TLightSchemeMapper _lightSchemeMapper = new();
    private readonly TDarkSchemeMapper _darkSchemeMapper = new();

    private bool _enableTheming;
    private bool _enableDynamicColor;
    private uint _seed;
    private uint? _prevSeed;
    private bool? _prevIsDark;

    public MaterialColorService(
        IOptions<MaterialColorOptions> options,
        IDynamicColorService dynamicColorService,
        IApplication application,
        IPreferences preferences)
    {
        _rememberIsDark = options.Value.RememberIsDark;
        _enableTheming = options.Value.EnableTheming;
        _enableDynamicColor = options.Value.EnableDynamicColor;
        _fallbackSeed = options.Value.FallbackSeed;
        
        _dynamicColorService = dynamicColorService;
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

    // Called by MauiAppBuilder.Build()
    public virtual void Initialize(IServiceProvider? services)
    {
        if (_preferences.ContainsKey(IsDarkKey))
            _application.UserAppTheme = _preferences.Get(IsDarkKey, false)
                ? AppTheme.Dark
                : AppTheme.Light;

        _seed = _fallbackSeed;
        
        if (_preferences.ContainsKey(SeedKey))
            _seed = (uint)_preferences.Get(SeedKey, 0);
        
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

        if (Seed != _prevSeed)
            CorePalette.Fill(Seed);

        if (_enableDynamicColor && _dynamicColorService.CorePalette != null)
        {
            CorePalette.Primary = _dynamicColorService.CorePalette.Primary;
            CorePalette.Secondary = _dynamicColorService.CorePalette.Secondary;
            CorePalette.Tertiary = _dynamicColorService.CorePalette.Tertiary;
            CorePalette.Neutral = _dynamicColorService.CorePalette.Neutral;
            CorePalette.NeutralVariant = _dynamicColorService.CorePalette.NeutralVariant;
        }

        if (Seed == _prevSeed && IsDark == _prevIsDark) return;
        _prevSeed = Seed;
        _prevIsDark = IsDark;
        
        ISchemeMapper<TCorePalette, TSchemeInt> mapper = IsDark
            ? _darkSchemeMapper
            : _lightSchemeMapper;

        SchemeInt = mapper.Map(CorePalette);

        if (typeof(TSchemeMaui) == typeof(Scheme<Color>))
        {
            SchemeMaui = (TSchemeMaui)SchemeInt.ConvertTo(Color.FromUint);
        }
        else
        {
            // We have to use reflection to access the generated method with the correct return type.
            SchemeMaui = (TSchemeMaui)typeof(TSchemeInt)
                .GetMethods()
                .Where(m => m.Name == nameof(Scheme<int>.ConvertTo))
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
        foreach (PropertyInfo property in typeof(TSchemeMaui).GetProperties())
        {
            string key = property.Name;
            Color value = (Color)property.GetValue(SchemeMaui)!;
            _appResources[key] = value;
            _appResources[key + "Brush"] = new SolidColorBrush(value);
        }
    }
}