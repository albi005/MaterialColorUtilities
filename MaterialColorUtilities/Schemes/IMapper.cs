using MaterialColorUtilities.Palettes;

namespace MaterialColorUtilities.Schemes
{
    public interface ISchemeMapper<TCorePalette, TScheme>
        where TCorePalette : CorePalette
        where TScheme : Scheme<int>, new()
    {
        TScheme Map(TCorePalette corePalette);
        TScheme Map(TCorePalette corePalette, TScheme scheme);
    }
}
