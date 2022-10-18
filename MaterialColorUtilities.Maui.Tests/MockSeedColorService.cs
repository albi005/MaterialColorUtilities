namespace MaterialColorUtilities.Maui.Tests;

public class MockSeedColorService : ISeedColorService
{
    public MockSeedColorService() { }
    
    public MockSeedColorService(int? seedColor)
    {
        SeedColor = seedColor;
    }

    public int? SeedColor { get; }
    public event Action? OnSeedColorChanged { add { } remove { } }
}