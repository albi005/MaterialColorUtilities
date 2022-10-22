namespace MaterialColorUtilities.Maui.Tests;

public class MockSeedColorService : ISeedColorService
{
    public MockSeedColorService() { }
    
    public MockSeedColorService(uint? seedColor)
    {
        SeedColor = seedColor;
    }

    public uint? SeedColor { get; }
    public event Action? OnSeedColorChanged { add { } remove { } }
}