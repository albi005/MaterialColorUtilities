namespace MaterialColorUtilities.Maui.Tests;

public class MockAccentColorService : IAccentColorService
{
    public MockAccentColorService() { }
    
    public MockAccentColorService(int? accentColor)
    {
        AccentColor = accentColor;
    }

    public int? AccentColor { get; }
    public event Action? OnAccentColorChanged { add { } remove { } }
}