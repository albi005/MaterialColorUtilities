using MaterialColorUtilities.Maui;
using MaterialColorUtilities.Palettes;
using Microsoft.Extensions.Options;
using Playground.Shared;
#if ANDROID
using Android.App;
using AndroidX.Core.View;
using Microsoft.Maui.Platform;
#endif

#pragma warning disable CS1998

namespace Playground.Maui;

public class CustomDynamicColorService : DynamicColorService<CorePalette, AppScheme<uint>, AppScheme<Color>, LightAppSchemeMapper, DarkAppSchemeMapper>
{
    private readonly WeakEventManager _weakEventManager = new();
    
    public CustomDynamicColorService(IOptions<DynamicColorOptions> options, ISeedColorService seedColorService, IApplication application, IPreferences preferences) : base(options, seedColorService, application, preferences)
    {
    }
    
    public event EventHandler SeedChanged
    {
        add => _weakEventManager.AddEventHandler(value);
        remove => _weakEventManager.RemoveEventHandler(value);
    }

    protected override async void Apply()
    {
        base.Apply();
        _weakEventManager.HandleEvent(null!, null!, nameof(SeedChanged));

#if ANDROID
        Activity activity = await Platform.WaitForActivityAsync();

        // Update status/navigation bar background color
        Android.Graphics.Color androidColor = SchemeMaui.Surface2.ToPlatform();
        activity.Window!.SetNavigationBarColor(androidColor);
        activity.Window!.SetStatusBarColor(androidColor);

        // Update status/navigation bar text/icon color
        _ = new WindowInsetsControllerCompat(activity.Window, activity.Window.DecorView)
        {
            AppearanceLightStatusBars = !IsDark,
            AppearanceLightNavigationBars = !IsDark
        };
#endif
    }
}
