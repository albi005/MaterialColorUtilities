namespace MaterialColorUtilities.Maui;

public interface IAccentColorService
{
    int? AccentColor { get; }
    event Action OnAccentColorChanged;
}