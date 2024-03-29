﻿using MaterialColorUtilities.Maui;
using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using Microsoft.Extensions.Options;
#if ANDROID
using Android.App;
using AndroidX.Core.View;
using Microsoft.Maui.Platform;
#endif

#pragma warning disable CS1998

namespace Playground.Maui;

public class CustomMaterialColorService : MaterialColorService<CorePalette, Scheme<uint>, Scheme<Color>, LightSchemeMapper, DarkSchemeMapper>
{
    private readonly WeakEventManager _weakEventManager = new();
    
    public CustomMaterialColorService(IOptions<MaterialColorOptions> options, IDynamicColorService dynamicColorService, IPreferences preferences) : base(options, dynamicColorService, preferences)
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
