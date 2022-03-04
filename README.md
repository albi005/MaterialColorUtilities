# Monet

.NET library for generating Material Color Palettes from an image or a color

Based on https://material-foundation.github.io/material-theme-builder/app.js

## Features

Generate a seed primary color from an image (for example the user's wallpaper):

```csharp
string imageId = "Monet.Samples.Assets.5_wallpaper.webp";
using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageId)!;
SKBitmap bitmap = SKBitmap.Decode(stream).Resize(new SKImageInfo(112,112), SKFilterQuality.Medium);
uint[] pixels = bitmap.Pixels.Select(p => (uint)p).ToArray();

uint seedColor = Utils.SeedFromImage(pixels);
```

Use that color to create a `CorePalette` that can be used to create any tone of any key color:

```csharp
CorePalette corePalette = new(seedColor);
Color color = corePalette.Secondary[55].ToColor();
```

Map the `CorePalette` to any 
[Material Design 3 named color](https://m3.material.io/styles/color/the-color-system/tokens)
using `Theme`

```csharp
Theme theme = new(corePalette);
Color primaryContainer = theme.PrimaryContainer;
```

Set `Theme.IsDark` to `true` to get the dark colors:

```csharp
theme.IsDark = true;
Color darkTertiary = theme.Tertiary;
```

`Theme` uses the 
[default Material Design 3 mapping of tokens](https://m3.material.io/styles/color/the-color-system/tokens#7fd4440e-986d-443f-8b3a-4933bff16646).
This can be changed by making your own theme:

```csharp
public class MyTheme : Theme
{
    public MyTheme(CorePalette corePalette) : base(corePalette) { }

    protected override uint GetBackgroundLight() => Palette.Primary[98];
    protected override uint GetSurfaceLight() => Palette.Neutral[100];
    protected override uint GetSurfaceDark() => Palette.Neutral[20];
}
```
