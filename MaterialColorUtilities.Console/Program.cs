using MaterialColorUtilities.Blend;
using MaterialColorUtilities.Console;
using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using MaterialColorUtilities.Utils;
using SkiaSharp;
using System.Drawing;
using System.Reflection;

// Extract a seed color from an image:
// Load the image into an int[].
// The image is stored in an embedded resource, and then decoded and resized using SkiaSharp.
string imageResourceId = "MaterialColorUtilities.Console.Resources.wallpaper.webp";
using Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageResourceId);
SKBitmap bitmap = SKBitmap.Decode(resourceStream).Resize(new SKImageInfo(112, 112), SKFilterQuality.Medium);
int[] pixels = bitmap.Pixels.Select(p => (int)(uint)p).ToArray();

// This is where the magic happens
int seedColor = ImageUtils.ColorsFromImage(pixels).First();

Console.WriteLine($"Seed: {StringUtils.HexFromArgb(seedColor)}");

// CorePalette gives you access to every tone of the key colors
CorePalette corePalette = new(seedColor);

// Map the core palette to color schemes
// A Scheme contains the named colors, like Primary or OnTertiaryContainer
Scheme<int> lightScheme = new LightSchemeMapper().Map(corePalette);
Scheme<int> darkScheme = new DarkSchemeMapper().Map(corePalette);

// Easily convert between Schemes with different color types
Scheme<Color> schemeColor = lightScheme.ConvertTo(Color.FromArgb);

Scheme<string> lightSchemeString = lightScheme.ConvertTo(x => "#" + x.ToString("X")[2..]);
ConsoleHelper.PrintProperties("Light scheme", lightSchemeString);

Scheme<string> darkSchemeString = darkScheme.ConvertTo(StringUtils.HexFromArgb);
ConsoleHelper.PrintProperties("Dark scheme", darkSchemeString);


// - EXTENSION -
// Adding your own colors:

// 4. Use your new colors (this is should be at the end, but you can't add top-level statements after type declarations)
MyCorePalette myCorePalette = new(seedColor);
MyScheme<string> myDarkScheme = new MyDarkSchemeMapper()
    .Map(myCorePalette)
    .ConvertTo(StringUtils.HexFromArgb);
ConsoleHelper.PrintProperties("My dark scheme", myDarkScheme);

// 1. Define a new key color if you need by subclassing CorePalette
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

// 2. Subclass Scheme
// The source generator will add new converter methods on build.
// Make sure to mark it partial.
public partial class MyScheme<TColor1> : Scheme<TColor1>
{
    public TColor1 Orange { get; set; }
    public TColor1 OnOrange { get; set; }
    public TColor1 OrangeContainer { get; set; }
    public TColor1 OnOrangeContainer { get; set; }
}

// 3. Create mappers
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
