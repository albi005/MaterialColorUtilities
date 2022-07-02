using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using Microsoft.Extensions.Options;

namespace MaterialColorUtilities.Maui;

public partial class DynamicColorService : IMauiInitializeService
{
    private readonly DynamicColorOptions _options;
    private readonly WeakEventManager _weakEventManager = new();
    private int _seed;
    private bool _initialized;
    private ResourceDictionary _appResources;

    public DynamicColorService(IOptions<DynamicColorOptions> options)
    {
        _options = options.Value;
        _seed = _options.FallbackSeed;
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

    public void Initialize(IServiceProvider services)
    {
        Application application = services.GetRequiredService<IApplication>() as Application;
        _appResources = application.Resources;

        _seed = _options.FallbackSeed;

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
