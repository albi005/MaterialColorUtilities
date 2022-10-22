namespace MaterialColorUtilities.Maui;

public interface ISeedColorService
{
    uint? SeedColor { get; }
    event Action OnSeedColorChanged;
}