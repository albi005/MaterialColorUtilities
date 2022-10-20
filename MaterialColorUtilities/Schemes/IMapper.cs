using MaterialColorUtilities.Palettes;

namespace MaterialColorUtilities.Schemes
{
    /// <summary>
    /// An interface for a generic mapper that maps a <typeparamref name="TCorePalette"/> to a <typeparamref name="TScheme"/>.
    /// </summary>
    /// <typeparam name="TCorePalette">The type of the CorePalette.</typeparam>
    /// <typeparam name="TScheme">The type of the Scheme.</typeparam>
    public interface ISchemeMapper<TCorePalette, TScheme>
        where TCorePalette : CorePalette
        where TScheme : Scheme<uint>, new()
    {
        /// <summary>
        /// Maps from a <typeparamref name="TCorePalette"/> to a <typeparamref name="TScheme"/>.
        /// </summary>
        /// <param name="corePalette">The source palette.</param>
        /// <returns>A new <typeparamref name="TScheme"/> with colors from <paramref name="corePalette"/>.</returns>
        TScheme Map(TCorePalette corePalette);

        /// <summary>
        /// Maps from a <typeparamref name="TCorePalette"/> to a <typeparamref name="TScheme"/>.
        /// </summary>
        /// <param name="corePalette">The source palette.</param>
        /// <param name="scheme">The scheme to modify.</param>
        /// <returns><paramref name="scheme"/> with colors replaced from <paramref name="corePalette"/>.</returns>
        TScheme Map(TCorePalette corePalette, TScheme scheme);
    }
}
