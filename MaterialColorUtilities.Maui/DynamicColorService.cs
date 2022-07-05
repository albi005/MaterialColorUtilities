using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using Microsoft.Extensions.Options;
using Microsoft.Maui.LifecycleEvents;

namespace MaterialColorUtilities.Maui;

public partial class DynamicColorService
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
    public CorePalette CorePalette { get; protected set; }
    public Scheme<int> SchemeInt { get; protected set; }
    public Scheme<Color> SchemeMaui { get; protected set; }

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

    private void Apply()
    {
        bool isDark = AppInfo.RequestedTheme == AppTheme.Dark;

        CorePalette = new(Seed);

        ISchemeMapper<CorePalette, Scheme<int>> mapper = isDark
            ? new DarkSchemeMapper()
            : new LightSchemeMapper();

        SchemeInt = mapper.Map(CorePalette);
        SchemeMaui = SchemeInt.ConvertTo(Color.FromInt);

        foreach (var property in SchemeMaui.GetType().GetProperties())
        {
            string key = property.Name;
            Color value = (Color)property.GetValue(SchemeMaui);
            _appResources[key] = value;
            _appResources[key + "Brush"] = new SolidColorBrush(value);
        }
    }
}
