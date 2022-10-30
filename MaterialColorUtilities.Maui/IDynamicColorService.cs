using MaterialColorUtilities.Palettes;

namespace MaterialColorUtilities.Maui;

public interface IDynamicColorService
{
    CorePalette? CorePalette => null;
    uint? SeedColor { get; }
    event Action Changed;
}