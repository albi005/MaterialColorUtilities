# MaterialColorUtilities.Maui

*A library for adding Material You colors to your .NET MAUI app*

## Features
- Dynamic theming on every platform (except iOS)
- Light/dark theme support
- Automatically storing and reapplying seed color/dark mode/style preferences

All of these can be turned on/off at any time.

## Getting started

1. Add a reference to the package. For instructions visit the [NuGet page](https://www.nuget.org/packages/MaterialColorUtilities.Maui).
2. Call the `UseMaterialColors` extension method in `MauiProgram.cs`. This will add all the services required for theming to the service container.
```diff
+using MaterialColorUtilities.Maui;
namespace YourApp;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp
            .CreateBuilder()
+           .UseMaterialColors()
            .UseMauiApp<App>();
        return builder.Build();
    }
}
```
You can specify the fallback seed as an argument to the extension method or use a lambda for more options:
```csharp
.UseMaterialColors(options =>
{
    options.FallbackSeed = 0xB000B5;
    options.UseDynamicColor = false;
})
```

3. Initialize MaterialColorService with a ResourceDictionary:

<details>
<summary>
If you use XAML click here
</summary>

Copy these lines:
```xml
<Color x:Key="Primary" />
<Color x:Key="PrimaryContainer" />
<Color x:Key="Secondary" />
<Color x:Key="SecondaryContainer" />
<Color x:Key="Tertiary" />
<Color x:Key="TertiaryContainer" />
<Color x:Key="Surface" />
<Color x:Key="SurfaceVariant" />
<Color x:Key="Background" />
<Color x:Key="Error" />
<Color x:Key="ErrorContainer" />
<Color x:Key="OnPrimary" />
<Color x:Key="OnPrimaryContainer" />
<Color x:Key="OnSecondary" />
<Color x:Key="OnSecondaryContainer" />
<Color x:Key="OnTertiary" />
<Color x:Key="OnTertiaryContainer" />
<Color x:Key="OnSurface" />
<Color x:Key="OnSurfaceVariant" />
<Color x:Key="OnError" />
<Color x:Key="OnErrorContainer" />
<Color x:Key="OnBackground" />
<Color x:Key="Outline" />
<Color x:Key="Shadow" />
<Color x:Key="InverseSurface" />
<Color x:Key="InverseOnSurface" />
<Color x:Key="InversePrimary" />
<Color x:Key="Surface1" />
<Color x:Key="Surface2" />
<Color x:Key="Surface3" />
<Color x:Key="Surface4" />
<Color x:Key="Surface5" />
<SolidColorBrush x:Key="PrimaryBrush" />
<SolidColorBrush x:Key="PrimaryContainerBrush" />
<SolidColorBrush x:Key="SecondaryBrush" />
<SolidColorBrush x:Key="SecondaryContainerBrush" />
<SolidColorBrush x:Key="TertiaryBrush" />
<SolidColorBrush x:Key="TertiaryContainerBrush" />
<SolidColorBrush x:Key="SurfaceBrush" />
<SolidColorBrush x:Key="SurfaceVariantBrush" />
<SolidColorBrush x:Key="BackgroundBrush" />
<SolidColorBrush x:Key="ErrorBrush" />
<SolidColorBrush x:Key="ErrorContainerBrush" />
<SolidColorBrush x:Key="OnPrimaryBrush" />
<SolidColorBrush x:Key="OnPrimaryContainerBrush" />
<SolidColorBrush x:Key="OnSecondaryBrush" />
<SolidColorBrush x:Key="OnSecondaryContainerBrush" />
<SolidColorBrush x:Key="OnTertiaryBrush" />
<SolidColorBrush x:Key="OnTertiaryContainerBrush" />
<SolidColorBrush x:Key="OnSurfaceBrush" />
<SolidColorBrush x:Key="OnSurfaceVariantBrush" />
<SolidColorBrush x:Key="OnErrorBrush" />
<SolidColorBrush x:Key="OnErrorContainerBrush" />
<SolidColorBrush x:Key="OnBackgroundBrush" />
<SolidColorBrush x:Key="OutlineBrush" />
<SolidColorBrush x:Key="ShadowBrush" />
<SolidColorBrush x:Key="InverseSurfaceBrush" />
<SolidColorBrush x:Key="InverseOnSurfaceBrush" />
<SolidColorBrush x:Key="InversePrimaryBrush" />
<SolidColorBrush x:Key="Surface1Brush" />
<SolidColorBrush x:Key="Surface2Brush" />
<SolidColorBrush x:Key="Surface3Brush" />
<SolidColorBrush x:Key="Surface4Brush" />
<SolidColorBrush x:Key="Surface5Brush" />
```

and update `App.xaml` like this:
```diff
<?xml version="1.0" encoding="UTF-8" ?>
<Application
    x:Class="Playground.Maui.App"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
+   xmlns:mcu="clr-namespace:MaterialColorUtilities.Maui;assembly=MaterialColorUtilities.Maui">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
+               <ResourceDictionary>
+                   <!-- Copied lines go here -->
+               </ResourceDictionary>
+               <mcu:MaterialColorResourceDictionary />

                <!-- ResourceDictionaries that use Material colors can go here -->
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
            </ResourceDictionary.MergedDictionaries>

            <!-- Your styles that use Material colors can go here -->
            <Style TargetType="Button">
                <Setter Property="BackgroundColor" Value="{DynamicResource Primary}" />
                <Setter Property="TextColor" Value="{DynamicResource OnPrimary}" />
            </Style>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

</details>

<details>
<summary>If you don't use XAML click here</summary>

Edit `App.xaml.cs` like this:

```diff
+using MaterialColorUtilities.Maui;

public class App : Application
{
    public App()
    {
+       IMaterialColorService.Current.Initialize(this.Resources);
        
        MainPage = new AppShell();
    }
}
```
</details>

### Usage
By default the colors are added as global resources. You can access them

- in XAML using DynamicResource:
```xml
<Button BackgroundColor="{DynamicResource Primary}" />
```

- in C# using `Element.SetDynamicResource()`:
```csharp
Button button = new();
button.SetDynamicResource(Button.BackgroundProperty, "Primary");
```

The `MaterialColorService` can be resolved from the MAUI dependency injection container. It contains the current core palette and schemes. It also has properties for updating the seed color and style.
```csharp
using MaterialColorUtilites.Maui;

public class MyService
{
    public MyService(MaterialColorService materialColorService)
    {
        Scheme<Color> scheme = materialColorService.SchemeMaui;
        materialColorService.Seed = 0x123456;
        materialColorService.Style = Style.Spritz;
    }
}
```

## Customization
Extending MaterialColorService allows you to use your custom colors or set different outputs for theming.

When you do this, register the new service at startup:
```diff
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp
            .CreateBuilder()
+           .UseMaterialColors<MyMaterialColorService>()
            .UseMauiApp<App>();
        return builder.Build();
    }
}
```

### Custom colors
If you want to use custom colors, follow the instructions [here](../MaterialColorUtilities/README.md#custom-colors) on how to create you own palettes/schemes. Then specify them as generic type arguments:
```csharp
public class MyMaterialColorService : MaterialColorService<MyCorePalette, MyScheme<uint>, MyScheme<Color>, MyLightSchemeMapper, MyDarkSchemeMapper>
{
    public MyMaterialColorService(IOptions<MaterialColorOptions> options, IDynamicColorService dynamicColorService, IPreferences preferences)
        : base(options, dynamicColorService, preferences)
    {
    }
}
```

### Custom theming
By overriding the Apply method you can define things that should happen when colors change. For example invoking an event, setting native colors or if you are using .NET MAUI with Blazor, updating CSS.

```csharp
public class MyMaterialColorService : MaterialColorService
{
    private readonly WeakEventManager _weakEventManager = new();
    
    public MyMaterialColorService(IOptions<MaterialColorOptions> options, IDynamicColorService dynamicColorService, IPreferences preferences) : base(options, dynamicColorService, preferences)
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
```

## Implementation details
Light/dark mode handling works everywhere, but the dynamic theming source is different on all of the platforms:
- Android ≥12.0 (API 31): use system colors
- Android ≥8.1 (API 27): use [WallpaperColors](https://developer.android.com/reference/android/app/WallpaperColors).Primary as seed color
- Android ≥7.0 (API 24): if the `StorageRead` permission has been granted, extract a color from the wallpaper, otherwise use fallback seed
- Android ≥5.0 (API 21): use fallback seed
- iOS: no accent color, use fallback seed
- Mac: use accent color
- Windows: use accent color
