using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using Microsoft.Extensions.Options;
using Microsoft.Maui.LifecycleEvents;
using System.Diagnostics.CodeAnalysis;

namespace MaterialColorUtilities.Maui;

public sealed class DynamicColorService : DynamicColorService<CorePalette, Scheme<int>, Scheme<Color>, LightSchemeMapper, DarkSchemeMapper>
{
    public DynamicColorService(
        IOptions<DynamicColorOptions> options,
        IApplication application,
        ILifecycleEventService lifecycleEventService
    ) : base(options, application, lifecycleEventService) { }
}

public partial class DynamicColorService<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    TCorePalette,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
    TSchemeInt,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    TSchemeMaui,
    TLightSchemeMapper,
    TDarkSchemeMapper>
    : IDynamicColorService
    where TCorePalette : CorePalette
    where TSchemeInt : Scheme<int>, new()
    where TSchemeMaui : Scheme<Color>, new()
    where TLightSchemeMapper : ISchemeMapper<TCorePalette, TSchemeInt>, new()
    where TDarkSchemeMapper : ISchemeMapper<TCorePalette, TSchemeInt>, new()
{
    private readonly DynamicColorOptions _options;
    private readonly Application _application;
    private readonly ResourceDictionary _appResources;
    private readonly LifecycleEventService _lifecycleEventService;
    private bool _initialized;
    private int _seed;
    private int _prevSeed;
    private bool _prevIsDark;
    private readonly WeakEventManager _weakEventManager = new();
    private readonly TLightSchemeMapper _lightSchemeMapper = new();
    private readonly TDarkSchemeMapper _darkSchemeMapper = new();

    public DynamicColorService(
        IOptions<DynamicColorOptions> options,
        IApplication application,
        ILifecycleEventService lifecycleEventService)
    {
        _options = options.Value;
        _seed = _options.FallbackSeed;
        _application = (Application)application;
        _appResources = _application.Resources;
        _lifecycleEventService = (LifecycleEventService)lifecycleEventService;
    }

    public int Seed => _seed;
    public TCorePalette CorePalette { get; protected set; }
    public TSchemeInt SchemeInt { get; protected set; }
    public TSchemeMaui SchemeMaui { get; protected set; }

    public void SetSeed(int value, object sender = null)
    {
        _seed = value;

        if (!_initialized) return;

        _weakEventManager.HandleEvent(sender, value, nameof(SeedChanged));
        Apply();
    }

    public event EventHandler<int> SeedChanged
    {
        add => _weakEventManager.AddEventHandler(value);
        remove => _weakEventManager.RemoveEventHandler(value);
    }

    public virtual void Initialize()
    {
        PlatformInitialize();

        _application.RequestedThemeChanged += (_, _) => Apply();

        Apply();
        _initialized = true;
    }

    partial void PlatformInitialize();

    protected virtual void Apply()
    {
        if (Seed != _prevSeed)
            CorePalette = CreateCorePalette(Seed);

        bool isDark = AppInfo.RequestedTheme == AppTheme.Dark;

        if (Seed == _prevSeed && isDark == _prevIsDark) return;
        _prevSeed = Seed;
        _prevIsDark = isDark;

        ISchemeMapper<TCorePalette, TSchemeInt> mapper = isDark
            ? _darkSchemeMapper
            : _lightSchemeMapper;

        SchemeInt = mapper.Map(CorePalette);

        // We have to use reflection to access the generated method with the correct return type.
        SchemeMaui = (TSchemeMaui)typeof(TSchemeInt)
            .GetMethods()
            .Where(m => m.Name == nameof(Scheme<int>.ConvertTo))
            .ToList()[0]
            .MakeGenericMethod(typeof(Color))
            .Invoke(SchemeInt, new[] { Color.FromInt });

        foreach (var property in typeof(TSchemeMaui).GetProperties())
        {
            string key = property.Name;
            Color value = (Color)property.GetValue(SchemeMaui);
            _appResources[key] = value;
            _appResources[key + "Brush"] = new SolidColorBrush(value);
        }
    }

    /// <summary>Constructs a <typeparamref name="TCorePalette"/>.</summary>
    /// <remarks>
    /// C# doesn't support contructor with parameters as a generic constraint,
    /// so reflection is required to access the constructor. <see href="https://github.com/dotnet/csharplang/discussions/769">Discussion here.</see>
    /// If you replace CorePalette, make sure it has a constructor with the following parameters: <c>int seed, bool isContent</c>
    /// </remarks>
    private static TCorePalette CreateCorePalette(int seed)
    {
        return (TCorePalette)Activator.CreateInstance(typeof(TCorePalette), seed, false);
    }
}

public interface IDynamicColorService
{
    void Initialize();
}