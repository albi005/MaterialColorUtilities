using MaterialColorUtilities.Palettes;

namespace MaterialColorUtilities.Maui.Tests;

public class MockDynamicColorService : IDynamicColorService
{
    public MockDynamicColorService() { }
    
    public MockDynamicColorService(uint? seedColor)
    {
        SeedColor = seedColor;
    }

    public CorePalette? CorePalette => null;
    public uint? SeedColor { get; }
    public event Action? Changed { add { } remove { } }
}