using Monet;
using SkiaSharp;
using System.Drawing;
using System.Reflection;

// Generate seed color from an image
string sourceImageResourceID = "Monet.Samples.Assets.5_wallpaper.webp";
using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sourceImageResourceID)!;
// For good results resize the image to something like 112x112
SKBitmap bitmap = SKBitmap.Decode(stream).Resize(new SKImageInfo(112,112), SKFilterQuality.Medium);
uint seedColor = Utils.SeedFromImage(bitmap.Pixels.Select(p => (uint)p).ToArray());
Console.WriteLine($"Seed: #{seedColor.ToString("X")[2..]}");

// CorePalette gives you access to every tone of the key colors
CorePalette corePalette = new(seedColor);

// Theme maps CorePalette to named colors
Theme theme = new(corePalette);

Console.WriteLine("\n============\nLight theme:\n============");
foreach (var property in typeof(Theme).GetProperties())
{
    if (property.PropertyType == typeof(Color) && !property.Name.EndsWith("Dark") && !property.Name.EndsWith("Light"))
        Console.WriteLine($"{property.Name}: #{((Color)property.GetValue(theme)!).ToArgb().ToString("X")[2..]}");
}

Console.WriteLine("\n============\nDark theme:\n============");
theme.IsDark = true;
foreach (var property in typeof(Theme).GetProperties())
{
    if (property.PropertyType == typeof(Color) && !property.Name.EndsWith("Dark") && !property.Name.EndsWith("Light"))
        Console.WriteLine($"{property.Name}: #{((Color)property.GetValue(theme)!).ToArgb().ToString("X")[2..]}");
}