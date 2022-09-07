namespace MaterialColorUtilities.Maui.Tests;

public class MockAccentColorService : IAccentColorService
{
    public int? AccentColor { get; }
    public event Action? OnAccentColorChanged;
}