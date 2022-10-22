using MaterialColorUtilities.Palettes;

namespace MaterialColorUtilities.Schemes
{
    /// <summary>
    /// Base class for mappers.
    /// </summary>
    public abstract class BaseSchemeMapper<TCorePalette, TScheme> : ISchemeMapper<TCorePalette, TScheme>
        where TCorePalette : CorePalette
        where TScheme : Scheme<uint>, new()
    {
        public TScheme Map(TCorePalette corePalette) => Map(corePalette, new());

        public TScheme Map(TCorePalette corePalette, TScheme scheme)
        {
            MapCore(corePalette, scheme);
            return scheme;
        }

        /// <summary>
        /// Does the actual mapping of the core palette to the scheme.
        /// Override to add more mapping statements.
        /// </summary>
        protected abstract void MapCore(TCorePalette corePalette, TScheme scheme);
    }
}
