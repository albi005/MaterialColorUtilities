# MaterialColorUtilities

*C# implementation of Google's [Material color utilities](https://github.com/material-foundation/material-color-utilities)*

## Features
- **HCT**: a color space similar to HSL but with perceptually accurate colors
- **Quantize and score**: extract the best colors from an image for theming
- **Scheme**: turn a single color into a palette for theming

Features not in Google's version:
- **Style**: different ways to generate palettes, available from Android 13
- A generic scheme that supports different color types and adding type-safe custom colors

## Walkthrough
### HCT
Everything in this library is built around the HCT color space. It was created by Google for Material You and can be converted to and from RGB. HCT enables measuring contrast just by comparing the two colors' HCT tone.

It has three components:
- **Hue** is where on the color wheel the color is; 0≤hue<360.
- **Chroma** is how colorful the color is; 0 is gray and it can go up to a value based on the hue and tone.
- **Tone** is lightness; 0 is black, 100 is white.

You can try out HCT and see how the different components affect the color on [this website made using this library](https://albi005.github.io/MaterialColorUtilities/). To learn more about how HCT was designed, read [The Science of Color & Design](https://material.io/blog/science-of-color-design).

Let's see some code:
```csharp
// Converting RGB to HCT
uint argbColor = 0x0000FF; // Blue
Hct hct = Hct.FromInt(argbColor);
Console.WriteLine(hct.Hue);    // 283
Console.WriteLine(hct.Chroma); // 87
Console.WriteLine(hct.Tone);   // 32

// All of the properties can be updated any time. This causes chroma to be recalculated.
hct.Tone = 90;
Console.WriteLine(hct.Hue);    // 282
Console.WriteLine(hct.Chroma); // 19
Console.WriteLine(hct.Tone);   // 90

// Converting HCT to RGB
Hct hct = Hct.From(hue, chroma, tone);
uint argbColor = Hct.ToInt(); // 0xFF0000FF
```

### Color palettes

> Before going any further make sure to read the [corresponding article on material.io](https://m3.material.io/styles/color/the-color-system/key-colors-tones).

A **tonal palette** can be used to access different tones of a color while also caching them:
```csharp
TonalPalette blue = TonalPalette.FromInt(0x0000FF);
uint tone69 = blue[69];
uint tone49_6 = blue[49.6];
```

A **core palette** holds the Primary, Secondary, Tertiary, Neutral, NeutralVariant and Error tonal palettes. You can also add your own tonal palettes by extending this class.

You can get a core palette using its static methods:
```csharp
CorePalette corePalette = CorePalette.Of(0x123456);
uint primary60 = corePalette.Primary[60];
```

Alternatively you can construct an empty one and then call the `Fill` method on it, passing in a seed color and a style.
```csharp
CorePalette corePalette = new();
corePalette.Fill(0x123456, Style.Expressive);
uint primary60 = corePalette.Primary[60];
```

A core palette can be turned into a color scheme using a mapper:
```csharp
Scheme<uint> scheme = new LightSchemeMapper().Map(corePalette);
```

You can change the color type by calling the `Convert` method and passing in a converter function:
```csharp
Scheme<string> schemeString = scheme.Convert(x => "#" + x.ToString("x6"));
Scheme<Color> schemeColor = scheme.Convert(Color.FromUint);
```

### Color extraction from an image

The function takes in an array of colors in AARRGGBB format and returns a few colors that are best suitable for being seed colors for theming.

```csharp
uint[] pixels = ...;
List<uint> bestColors = ImageUtils.ColorsFromImage(pixels);
uint seed = bestColors[0];
```

### Custom colors

Subclass CorePalette, add a TonalPalette property and override the Fill method to give the tonal palette value:
```csharp
public class MyCorePalette : CorePalette
{
    public TonalPalette Orange { get; set; }

    public override void Fill(uint seed, Style style = Style.TonalSpot)
    {
        base.Fill(seed, style);

        // You can harmonize a color to make it closer to the seed color
        uint harmonizedOrange = Blender.Harmonize(0xFFA500, seed);
        Orange = TonalPalette.FromInt(harmonizedOrange);
    }
}
```

Then subclass Scheme:
```csharp
public partial class MyScheme<TColor> : Scheme<TColor>
{
    public TColor Orange { get; set; }
    public TColor OnOrange { get; set; }
    public TColor OrangeContainer { get; set; }
    public TColor OnOrangeContainer { get; set; }
}
```
A source generator will generate new Convert methods automatically as another part of this class.  
Make sure to **mark the class `partial`** and don't nest it inside another class.  
If you get warning `CS8032: An instance of analyzer MaterialColorUtilities.Schemes.SchemeConverterGenerator cannot be created...` your IDE/compiler doesn't have Roslyn 4.0, so the source generator won't work. Make sure you are using Visual Studio 2022, MSBuild 17 or .NET 6.

Finally, subclass the mappers, override the MapCore method and define how to map the core palette to the scheme:
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

Use them like this:
```csharp
MyCorePalette myCorePalette = new();
myCorePalette.Fill(seedColor);
MyScheme<uint> myDarkScheme = new MyDarkSchemeMapper().Map(myCorePalette);
MyScheme<string> myDarkScheme = myDarkScheme.Convert(StringUtils.HexFromArgb);
```
