using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using MaterialColorUtilities.Utils;
using SkiaSharp;
using System.Drawing;
using System.Reflection;

// Generate seed color from an image:
// Load the image into an int[].
// The image is stored in an embedded resource, and then decoded and resized using SkiaSharp.
string imageResourceId = "MaterialColorUtilities.Console.Resources.wallpaper.webp";
using Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageResourceId)!;
SKBitmap bitmap = SKBitmap.Decode(resourceStream).Resize(new SKImageInfo(112, 112), SKFilterQuality.Medium);
int[] pixels = bitmap.Pixels.Select(p => (int)(uint)p).ToArray();

// This is where the magic happens
int seedColor = ImageUtils.ColorFromImage(pixels);

Console.WriteLine($"Seed: #{seedColor.ToString("X")[2..]}");

// CorePalette gives you access to every tone of the key colors
CorePalette corePalette = CorePalette.Of(seedColor);

Console.WriteLine("\n============\nLIGHT\n============");
LightScheme lightScheme = new(corePalette);
foreach (var property in typeof(Scheme<int>).GetProperties())
{
    int color = (int)property.GetValue(lightScheme)!;
    Console.WriteLine($"{property.Name}: #{color.ToString("X")[2..]}");
}

Console.WriteLine("\n============\nDARK\n============");
DarkScheme darkScheme = new(corePalette);
foreach (var property in typeof(Scheme<int>).GetProperties())
{
    int color = (int)property.GetValue(darkScheme)!;
    Console.WriteLine($"{property.Name}: #{color.ToString("X")[2..]}");
}

// Convert:
Scheme<Color> colorScheme = lightScheme.Convert(Color.FromArgb);