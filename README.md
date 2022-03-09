# Material Color Utilities for .NET

This is a C# implementation of Google's [Material Color Utilities](https://github.com/material-foundation/material-color-utilities), that can be used to extract a color from an image and then generate a Material Design 3 color scheme.

## Install

Get the package from Nuget:
[![NuGet](https://img.shields.io/nuget/v/MaterialColorUtilities.svg)](https://www.nuget.org/packages/MaterialColorUtilities)

## Quickstart

Generate a seed color from an image (e.g. the user's wallpaper):

```csharp
// Load the image into an int[].
// Here, the image is stored in an embedded resource, and then decoded and resized using SkiaSharp.
// You might have to implement this part, according to your needs.
string imageResourceId = "MaterialColorUtilities.Samples.Assets.5_wallpaper.webp";
using Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageResourceId)!;
SKBitmap bitmap = SKBitmap.Decode(resourceStream).Resize(new SKImageInfo(112, 112), SKFilterQuality.Medium);
int[] pixels = bitmap.Pixels.Select(p => (int)(uint)p).ToArray();

// This is where the magic happens
int seedColor = ImageUtils.ColorFromImage(pixels);
```

Use that color to create a `CorePalette` that can be used to create any tone of any key color:

```csharp
CorePalette corePalette = CorePalette.Of(seedColor);
int color = corePalette.Secondary[55];
```

Map the `CorePalette` to any 
[Material Design 3 named color](https://m3.material.io/styles/color/the-color-system/tokens)
using `Scheme`

```csharp
Scheme<int> scheme = new LightScheme(corePalette);
int tertiary = scheme.Tertiary;
```

A `Scheme` is generic so you can use any color type.
Convert using an extension method:

```csharp
Scheme<Drawing.Color> dcScheme = scheme.Convert(Drawing.Color.FromArgb);
```


`LightScheme` and `DarkScheme` use the 
[default Material Design 3 mapping of tokens](https://m3.material.io/styles/color/the-color-system/tokens#7fd4440e-986d-443f-8b3a-4933bff16646).
This can be changed by overriding them:

```csharp
public class MyScheme : LightScheme
{
    public MyScheme(CorePalette corePalette) : base(corePalette) { }

    protected override int BackgroundLight => Palette.Primary[98];
    protected override int SurfaceLight => Palette.Neutral[100];
    protected override int SurfaceDark => Palette.Neutral[20];
}
```

For more info check out the source code and the examples.