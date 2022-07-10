using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using Microsoft.Extensions.Options;
using Microsoft.Maui.LifecycleEvents;

namespace MaterialColorUtilities.Maui;

public sealed class DynamicColorService : DynamicColorService<CorePalette, Scheme<int>, Scheme<Color>, LightSchemeMapper, DarkSchemeMapper>
{
    public DynamicColorService(
        IOptions<DynamicColorOptions> options,
        IApplication application,
        ILifecycleEventService lifecycleEventService
    ) : base(options, application, lifecycleEventService) { }
}

public partial class DynamicColorService<TCorePalette, TSchemeInt, TSchemeMaui, TLightSchemeMapper, TDarkSchemeMapper>
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
    private int _seed;
    private bool _initialized;
    private readonly WeakEventManager _weakEventManager = new();

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

        Apply();
        _initialized = true;
    }

    partial void PlatformInitialize();

    protected virtual void Apply()
    {
        bool isDark = AppInfo.RequestedTheme == AppTheme.Dark;

        CorePalette = (TCorePalette)Activator.CreateInstance(typeof(TCorePalette), Seed, false);

        ISchemeMapper<TCorePalette, TSchemeInt> mapper = isDark
            ? new TDarkSchemeMapper()
            : new TLightSchemeMapper();

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
}

public interface IDynamicColorService
{
    void Initialize();
}