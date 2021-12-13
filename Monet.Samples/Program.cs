using Monet;
using System.Drawing;

CorePalette corePalette = new(0x2E510B);

Console.WriteLine($"Primary: \t{corePalette.Primary}");
Console.WriteLine($"Secondary: \t{corePalette.Secondary}");
Console.WriteLine($"Tertiary: \t{corePalette.Tertiary}");
Console.WriteLine($"Error: \t\t{corePalette.Error}");
Console.WriteLine($"Neutral: \t{corePalette.Neutral}");
Console.WriteLine($"NeutralVariant: {corePalette.NeutralVariant}");

Console.WriteLine();

Theme theme = new(corePalette);
theme.IsDark = false;
foreach (var property in typeof(Theme).GetProperties())
{
    if (property.PropertyType == typeof(Color) && !property.Name.EndsWith("Dark") && !property.Name.EndsWith("Light"))
        Console.WriteLine($"{property.Name}: #{((Color)property.GetValue(theme)!).ToArgb().ToString("X")[2..]}");
}
