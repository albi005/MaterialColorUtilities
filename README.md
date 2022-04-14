
# Material Color Utilities for .NET

This is a C# implementation of Google's [Material Color Utilities](https://github.com/material-foundation/material-color-utilities), that can be used to extract a color from an image and then generate a Material Design 3 color scheme. Also includes helpers for working with schemes.

## Install

Get the package from Nuget:
[![NuGet](https://img.shields.io/nuget/v/MaterialColorUtilities.svg)](https://www.nuget.org/packages/MaterialColorUtilities)

## Quickstart

Generate a seed color from an image (e.g. the user's wallpaper):

```csharp
// Load the image into an int[].
// The image is stored in an embedded resource, and then decoded and resized using SkiaSharp.
string imageResourceId = "MaterialColorUtilities.Console.Resources.wallpaper.webp";
using Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageResourceId);
SKBitmap bitmap = SKBitmap.Decode(resourceStream).Resize(new SKImageInfo(112, 112), SKFilterQuality.Medium);
int[] pixels = bitmap.Pixels.Select(p => (int)(uint)p).ToArray();

// This is where the magic happens
int seedColor = ImageUtils.ColorsFromImage(pixels).First();
```

Use that color to create a `CorePalette` that can be used to create any tone (0-100) of any key color:

```csharp
CorePalette corePalette = new(seedColor);
int color = corePalette.Secondary[69];
```

A *scheme* contains the [Material Design 3 named colors](https://m3.material.io/styles/color/the-color-system/tokens), like OnPrimary, Background, or TertiaryContainer. Turn the `CorePalette` into a `Scheme<int>` using a mapper:

```csharp
Scheme<int> lightScheme = new LightSchemeMapper().Map(corePalette);
Scheme<int> darkScheme = new DarkSchemeMapper().Map(corePalette);
```

Then convert your scheme to one with a different color type:

```csharp
Scheme<Color> lightSchemeColor = lightScheme.ConvertTo(Color.FromArgb);
Scheme<string> lightSchemeString = lightScheme.ConvertTo(x => "#" + x.ToString("X")[2..]);
```

## Extension
### Adding your own colors

1. Define a new *key color* if you need by subclassing `CorePalette`:

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

> 🤖 A source generator will add new converter methods automatically.
> 
> Make sure to mark the class `partial` and don't nest it inside another class.
> 
> If you get warning `CS8032: An instance of analyzer MaterialColorUtilities.Schemes.SchemeConverterGenerator cannot be created from...`
> your IDE/compiler doesn't have Rosyln 4.0, so the source generator won't work.
> Make sure you are using Visual Studio 2022+ (as it has MSBuild 17) or .NET SDK 6+.

```csharp
public partial class MyScheme<TColor> : Scheme<TColor>
{
    public TColor Orange { get; set; }
    public TColor OnOrange { get; set; }
    public TColor OrangeContainer { get; set; }
    public TColor OnOrangeContainer { get; set; }
}
```

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

4. Profit!

```csharp
MyCorePalette myCorePalette = new(seedColor);
MyScheme<string> myDarkScheme = new MyDarkSchemeMapper()
    .Map(myCorePalette)
    .ConvertTo(StringUtils.HexFromArgb);
```

For more info check out the source code and the example projects.
