namespace MaterialColorUtilities.Maui;

public class DynamicColorOptions
{
    /// <summary>
    /// Will be used if a dynamic accent color is not available.
    /// </summary>
    public int FallbackSeed { get; set; } = unchecked((int)0xff4285F4); // Google Blue
    
    /// <summary>
    /// Whether to use wallpaper/accent color based dynamic theming.
    /// </summary>
    /// <remarks>
    /// When set to <see langword="false"/>, <see cref="FallbackSeed"/> will be used as seed,
    /// even on platforms that expose an accent color.
    /// </remarks>
    public bool UseDynamicColor { get; set; } = true;
}
