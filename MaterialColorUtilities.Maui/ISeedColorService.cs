namespace MaterialColorUtilities.Maui;

public interface ISeedColorService
{
    int? SeedColor { get; }
    event Action OnSeedColorChanged;
}