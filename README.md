
# Material Color Utilities for .NET

*C# implementation of Google's [Material Color Utilities](https://github.com/material-foundation/material-color-utilities)*

Includes all of the algorithms you might need for adding Material You colors to your .NET app:
- [.NET MAUI](#net-maui): add beautiful Material You dynamic theming to your .NET MAUI app in just a *single line of code*
- [HCT](#hct): a new color space designed by Google for perfect contrasts, properties: *Hue*, *Chroma* and *Tone*
- [Palettes](#palettes): a [`TonalPalette`](#tonal-palette) allows easy access to the tones of a color, a [`CorePalette`](#core-palette) contains the 6 key tonal palettes: *Primary*, *Secondary*, *Tertiary*, *Neutral*, *NeutralVariant* and *Error*
- [Scheme](#scheme): map the colors in a Core Palette to roles, like `Primary`, `TertiaryContainer` or `OnError`
- [Quantize](#quantize): reduce the number of unique colors in an image to just 128
- [Score](#score): order those colors based on suitability for theming
- [Blend](#blend): shift a color towards the theme hue
- [Extension](#extension-1): add custom colors, override mappings

Other stuff:
- [More info](#more-info)
- [Useful links](#useful-links)
- [Contributing](#contributing)

## Install

Get the packages from Nuget:

| Package | Description | Link |
|---|---|---|
| `MaterialColorUtilites` | Contains all of the color algorithms and helpers | [![NuGet](https://img.shields.io/nuget/v/MaterialColorUtilities.svg)](https://www.nuget.org/packages/MaterialColorUtilities) |
| `MaterialColorUtilites.Maui` | Adds dynamic colors to your .NET MAUI app | [![NuGet](https://img.shields.io/nuget/v/MaterialColorUtilities.Maui.svg)](https://www.nuget.org/packages/MaterialColorUtilities.Maui) |

## .NET MAUI
Adding beautiful Material You dynamic colors to your app is super simple, just follow these instructions. The library will handle everything else. 

### Implementation
Light/dark mode handling works on every platform, but accent color is different on every platform:
- Android 12+: use system colors
- Android 8.1+: use WallpaperColors.Primary as seed color
- Android <8.1: if `StorageRead` permission is granted, extract a color from the wallpaper, otherwise use the default seed
- iOS: no accent color, so use default seed
- Mac: use accent color
- Windows: use accent color

### Setup
1. Add the `MaterialColorUtilites.Maui` package using the link above
2. Add the the highlighted lines to your `MauiProgram.cs`
```diff
+using MaterialColorUtilities.Maui;

namespace YourBeautifulApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp
            .CreateBuilder()
+           .UseMaterialDynamicColors()
            .UseMauiApp<App>();

        return builder.Build();
    }
}
```
3. (optional) Add placeholders to your `App.xaml` for suggestions when writing XAML
```diff
<?xml version = "1.0" encoding = "UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:YourBeautifulApp"
             x:Class="YourBeautifulApp.App">
    <Application.Resources>
+       <Color x:Key="Primary" />
+       <Color x:Key="PrimaryContainer" />
+       <Color x:Key="Secondary" />
+       <Color x:Key="SecondaryContainer" />
+       <Color x:Key="Tertiary" />
+       <Color x:Key="TertiaryContainer" />
+       <Color x:Key="Surface" />
+       <Color x:Key="SurfaceVariant" />
+       <Color x:Key="Background" />
+       <Color x:Key="Error" />
+       <Color x:Key="ErrorContainer" />
+       <Color x:Key="OnPrimary" />
+       <Color x:Key="OnPrimaryContainer" />
+       <Color x:Key="OnSecondary" />
+       <Color x:Key="OnSecondaryContainer" />
+       <Color x:Key="OnTertiary" />
+       <Color x:Key="OnTertiaryContainer" />
+       <Color x:Key="OnSurface" />
+       <Color x:Key="OnSurfaceVariant" />
+       <Color x:Key="OnError" />
+       <Color x:Key="OnErrorContainer" />
+       <Color x:Key="OnBackground" />
+       <Color x:Key="Outline" />
+       <Color x:Key="Shadow" />
+       <Color x:Key="InverseSurface" />
+       <Color x:Key="InverseOnSurface" />
+       <Color x:Key="InversePrimary" />
+       <Color x:Key="Surface1" />
+       <Color x:Key="Surface2" />
+       <Color x:Key="Surface3" />
+       <Color x:Key="Surface4" />
+       <Color x:Key="Surface5" />
+       <SolidColorBrush x:Key="PrimaryBrush" />
+       <SolidColorBrush x:Key="PrimaryContainerBrush" />
+       <SolidColorBrush x:Key="SecondaryBrush" />
+       <SolidColorBrush x:Key="SecondaryContainerBrush" />
+       <SolidColorBrush x:Key="TertiaryBrush" />
+       <SolidColorBrush x:Key="TertiaryContainerBrush" />
+       <SolidColorBrush x:Key="SurfaceBrush" />
+       <SolidColorBrush x:Key="SurfaceVariantBrush" />
+       <SolidColorBrush x:Key="BackgroundBrush" />
+       <SolidColorBrush x:Key="ErrorBrush" />
+       <SolidColorBrush x:Key="ErrorContainerBrush" />
+       <SolidColorBrush x:Key="OnPrimaryBrush" />
+       <SolidColorBrush x:Key="OnPrimaryContainerBrush" />
+       <SolidColorBrush x:Key="OnSecondaryBrush" />
+       <SolidColorBrush x:Key="OnSecondaryContainerBrush" />
+       <SolidColorBrush x:Key="OnTertiaryBrush" />
+       <SolidColorBrush x:Key="OnTertiaryContainerBrush" />
+       <SolidColorBrush x:Key="OnSurfaceBrush" />
+       <SolidColorBrush x:Key="OnSurfaceVariantBrush" />
+       <SolidColorBrush x:Key="OnErrorBrush" />
+       <SolidColorBrush x:Key="OnErrorContainerBrush" />
+       <SolidColorBrush x:Key="OnBackgroundBrush" />
+       <SolidColorBrush x:Key="OutlineBrush" />
+       <SolidColorBrush x:Key="ShadowBrush" />
+       <SolidColorBrush x:Key="InverseSurfaceBrush" />
+       <SolidColorBrush x:Key="InverseOnSurfaceBrush" />
+       <SolidColorBrush x:Key="InversePrimaryBrush" />
+       <SolidColorBrush x:Key="Surface1Brush" />
+       <SolidColorBrush x:Key="Surface2Brush" />
+       <SolidColorBrush x:Key="Surface3Brush" />
+       <SolidColorBrush x:Key="Surface4Brush" />
+       <SolidColorBrush x:Key="Surface5Brush" />
    </Application.Resources>
</Application>
```

### Options
Specify a fallback seed color as an argument to the extension method or use a lambda for more options:
```csharp
.UseMaterialDynamicColors(options =>
{
    options.FallbackSeed = unchecked((int)0xFFB000B5);
    options.UseDynamicColor = false;
})
```

### Usage
- In XAML:
```xml
<Button Color="{DynamicResource Primary}" />
```

- In C# views:
```csharp
Button button = new();
button.SetDynamicResource(Button.BackgroundColorProperty, MaterialColorUtilities.Schemes.Keys.Primary);
```

- You can also resolve `DynamicColorService` from the MAUI dependency injection container:
```csharp
using MaterialColorUtilites.Maui;

public class MyService
{
    public MyService(DynamicColorService dynamicColorService)
    {
        Scheme<Color> scheme = dynamicColorService.SchemeMaui;
    }
}

```
> For samples check out the [Playground.Maui](https://github.com/albi005/MaterialColorUtilities/tree/main/Playground/Playground.Maui) project.

### Extension
Using [custom components](#extension-1) (like CorePalette, schemes and mappers) is supported. Just subclass `DynamicColorService` and supply your own types as generic arguments. 
```csharp
public class MyDynamicColorService : DynamicColorService<MyCorePalette, MyScheme<int>, MyScheme<Color>, MyLightSchemeMapper, MyDarkSchemeMapper>
{
    // You can also override Initialize() and Apply() for custom logic
}
```
Then use it using the generic extension method:
```csharp
.UseMaterialDynamicColors<MyDynamicColorService>()
```

## HCT
Google's new color space designed for meeting contrast standards easy. Properties:
- **H**ue: 0-360, the color, like red, orange, yellow, green, blue or violet
- **C**hroma: how colorful the color is. 0 is gray, and the maximum depends on Hue and Tone.
- **T**one: lightness, 0-100, 0 is black, 100 is white

Convert an RGB color to HCT, change its Tone, then convert back:
```csharp
int argb = Colors.Azure.ToInt();
Hct hct = Hct.FromInt(argb);
hct.Tone = 30;
int tone30 = hct.ToInt();
```
> Check out the Blazor playground running live [here](https://albi005.github.io/MaterialColorUtilities/) or [Material Theme Builder](https://material-foundation.github.io/material-theme-builder/) for visualisations.

## [Palettes](https://github.com/albi005/MaterialColorUtilities/tree/main/MaterialColorUtilities/Palettes)
### [Tonal palette](https://github.com/albi005/MaterialColorUtilities/blob/main/MaterialColorUtilities/Palettes/TonalPalette.cs)
Used for retrieving tones of a color.
![](https://lh3.googleusercontent.com/X7h-ccmRcbYfSo-E_YnapoUQAHlNqWtrnSwRPJ4Pd020NPa753NbKfTZfFWiHpEkGIcITeTY605XJAw7yX4I1C7yAL0j44_H7KWLFsk_IW8=s0)
```csharp
// Does the same as the above code
TonalPalette palette = TonalPalette.FromInt(argb);
int tone30 = palette[30];
```

### [Core palette](https://github.com/albi005/MaterialColorUtilities/blob/main/MaterialColorUtilities/Palettes/CorePalette.cs)
Contains the Tonal palettes of the [key colors](https://m3.material.io/styles/color/the-color-system/key-colors-tones#5fdf196d-1e21-4d03-ae63-e802d61ad5ee).
![](https://lh3.googleusercontent.com/G_2Z3lRMdADfzbQyJZcZFAv61QpImyb9OhdmEpu_lAaxgPa01iY-QHPhIgCbkqPQTn9C4Jwzr2OufMQSmPcwJnmSdkmpmix_8HSrctOUjVo=s0)
Properties:
```csharp
public TonalPalette Primary { get; set; }
public TonalPalette Secondary { get; set; }
public TonalPalette Tertiary { get; set; }
public TonalPalette Neutral { get; set; }
public TonalPalette NeutralVariant { get; set; }
public TonalPalette Error { get; set; }
```

> Check out the [source code](https://github.com/albi005/MaterialColorUtilities/blob/main/MaterialColorUtilities/Palettes/CorePalette.cs#L45) if you want to know how they get constructed.

## [Scheme](https://github.com/albi005/MaterialColorUtilities/tree/main/MaterialColorUtilities/Schemes)
`Scheme`s store the Material Design 3 [color role](https://m3.material.io/styles/color/the-color-system/color-roles) values.
![](https://lh3.googleusercontent.com/nQHmWgLpXxjfV9nC_xIabgJDagi5V3aBB9qbFRA_EHEkEeTaq3uh-rYwoXnkRqL1eHCobVjb8lmQgdistb_XNcCfVdsQqUC-h0hvje4j6Qk=s0)
```csharp
public class Scheme<TColor>
{
    public TColor Primary { get; set; }
    public TColor OnPrimary { get; set; }
    public TColor PrimaryContainer { get; set; }
    public TColor OnPrimaryContainer { get; set; }
    public TColor Secondary { get; set; }
    public TColor OnSecondary { get; set; }
    public TColor SecondaryContainer { get; set; }
    ...
}
```

`Scheme`s are generic so you can use whatever color model you want.

This library uses "mappers" to turn `ColorPalette`s into `Scheme`s.
```csharp
int seed = unchecked((int)0xFF5D5FDB);

// Construct a CorePalette
CorePalette corePalette = CorePalette.Of(seed);

// Get a mapper
DarkSchemeMapper mapper = new();

// Map
Scheme<int> scheme = mapper.Map(corePalette);

// Convert color type
Scheme<Color> schemeMaui = scheme.Convert(Color.FromInt);
Scheme<string> schemeString = scheme.Convert(x => "#" + x.ToString("X")[2..]);
```

> For [custom colors](https://m3.material.io/styles/color/the-color-system/custom-colors), roles or mapping check out [Extension](#extension-1).

## [Quantize](https://github.com/albi005/MaterialColorUtilities/tree/main/MaterialColorUtilities/Quantize)
Reduce the number of unique colors in an image. Multiple algorithms are available and they are used together.

## [Score](https://github.com/albi005/MaterialColorUtilities/tree/main/MaterialColorUtilities/Score)
Order colors returned by quantization based on suitability for theming and remove similar ones.

## [Blend](https://github.com/albi005/MaterialColorUtilities/tree/main/MaterialColorUtilities/Blend)
Shift a color towards the hue of the seed color.
```csharp
using MaterialColorUtilities.Blend;
int harmonized = Blender.Harmonize(color, seedColor);
```

## Putting it all together
Generate a seed color from an image (e.g. the user's wallpaper):
```csharp
// Load the image into an int[].
// The image is stored in a resource embedded in the assembly, and then decoded and resized using SkiaSharp.
string imageResourceId = "MaterialColorUtilities.Console.Resources.wallpaper.webp";
using Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageResourceId);
SKBitmap bitmap = SKBitmap.Decode(resourceStream).Resize(new SKImageInfo(112, 112), SKFilterQuality.Medium);
int[] pixels = bitmap.Pixels.Select(p => (int)(uint)p).ToArray();

// Run quantization and scoring and use the best color
int seedColor = ImageUtils.ColorsFromImage(pixels).First();
```

Use that color to create a `CorePalette`;

```csharp
CorePalette corePalette = new(seedColor);
int color = corePalette.Secondary[69];
```

Turn the `CorePalette` into a `Scheme<int>` using a mapper:

```csharp
Scheme<int> lightScheme = new LightSchemeMapper().Map(corePalette);
Scheme<int> darkScheme = new DarkSchemeMapper().Map(corePalette);
```

Then convert your scheme to one with a different color type:

```csharp
using Color = System.Drawing.Color;
Scheme<Color> lightSchemeColor = lightScheme.ConvertTo(Color.FromArgb);
Scheme<string> lightSchemeString = lightScheme.ConvertTo(x => "#" + x.ToString("X")[2..]);
```
> The same code can be found in the [Playground.Console](https://github.com/albi005/MaterialColorUtilities/tree/main/Playground/Playground.Console) project.

## Extension
### Adding [custom colors](https://m3.material.io/styles/color/the-color-system/custom-colors)

1. Define a custom key color if you need by subclassing `CorePalette`:

```csharp
public class MyCorePalette : CorePalette
{
    public TonalPalette Orange { get; set; }
    
    public MyCorePalette(int seed) : base(seed)
    {
        // You can harmonize a color to make it closer to the seed color
        int harmonizedOrange = Blender.Harmonize(unchecked(0xFFA500), seed);
        Orange = TonalPalette.FromInt(harmonizedOrange);
    }
}
```

2. Subclass `Scheme<TColor>`

```csharp
public partial class MyScheme<TColor> : Scheme<TColor>
{
    public TColor Orange { get; set; }
    public TColor OnOrange { get; set; }
    public TColor OrangeContainer { get; set; }
    public TColor OnOrangeContainer { get; set; }
}
```

> A source generator will generate new converter methods automatically.
> 
> Make sure to mark the class `partial` and don't nest it inside another class.
> 
> If you get warning `CS8032: An instance of analyzer MaterialColorUtilities.Schemes.SchemeConverterGenerator cannot be created...` your IDE/compiler doesn't have Rosyln 4.0, so the source generator won't work. Make sure you are using Visual Studio 2022, MSBuild 17 or .NET 6.

3. Create mappers

```csharp
public class MyLightSchemeMapper : LightSchemeMapper<MyCorePalette, MyScheme<int>>
{
    protected override void MapCore(MyCorePalette palette, MyScheme<int> scheme)
    {
        base.MapCore(palette, scheme);
        scheme.Orange = palette.Orange[40];
        scheme.OnOrange = palette.Orange[100];
        scheme.OrangeContainer = palette.Orange[90];
        scheme.OnOrangeContainer = palette.Orange[10];

        // You can also override already mapped colors
        scheme.Surface = palette.Neutral[100];
    }
}

public class MyDarkSchemeMapper : DarkSchemeMapper<MyCorePalette, MyScheme<int>>
{
    protected override void MapCore(MyCorePalette palette, MyScheme<int> scheme)
    {
        base.MapCore(palette, scheme);
        scheme.Orange = palette.Orange[80];
        scheme.OnOrange = palette.Orange[20];
        scheme.OrangeContainer = palette.Orange[30];
        scheme.OnOrangeContainer = palette.Orange[90];
    }
}
```

4. Use your awesome new code

```csharp
MyCorePalette myCorePalette = new(seedColor);

MyScheme<string> myDarkScheme = new MyDarkSchemeMapper()
    .Map(myCorePalette)
    .ConvertTo(StringUtils.HexFromArgb);
```

> The same code can be found in the [Playground.Console](https://github.com/albi005/MaterialColorUtilities/tree/main/Playground/Playground.Console) project.

## More info
For more info and samples check out the source code and the playground projects. If you have questions use the [Discussions](https://github.com/albi005/MaterialColorUtilities/discussions) tab.

## Useful links:

- [Blazor sample](https://albi005.github.io/MaterialColorUtilities/) (Playground.Wasm project) on the web
  - Useful for comparing color spaces and checking HCT color values
- [Material Theme Builder](https://material-foundation.github.io/material-theme-builder/#/custom)
  - For creating nice themes

## Contributing
If you have found a bug or need a new feature, go ahead and open an [issue](https://github.com/albi005/MaterialColorUtilities/issues).
