using MaterialColorUtilities.Palettes;

namespace MaterialColorUtilities.Schemes
{
    public abstract class BaseSchemeMapper<TCorePalette, TScheme> : ISchemeMapper<TCorePalette, TScheme>
        where TCorePalette : CorePalette
        where TScheme : Scheme<int>, new()
    {
        public TScheme Map(TCorePalette corePalette) => Map(corePalette, new());

        public TScheme Map(TCorePalette corePalette, TScheme scheme)
        {
            MapCore(corePalette, scheme);
            return scheme;
        }

        protected abstract void MapCore(TCorePalette corePalette, TScheme scheme);
    }
}
