using Style = MaterialColorUtilities.Palettes.Style;

namespace MaterialColorUtilities.Maui;

public sealed class MaterialColorOptions
{
    /// <summary>
    /// When true, updates to Application.UserAppTheme will be stored using Preferences
    /// and reapplied when the app restarts. Defaults to true.
    /// </summary>
    public bool RememberIsDark { get; set; } = true;

    /// <summary>
    /// Determines whether to generate colors and add them to the global app resources.
    /// Set to false to disable theming.
    /// </summary>
    /// <remarks>
    /// Can be updated at runtime using MaterialColorService.IsEnabled.
    /// </remarks>
    public bool EnableTheming { get; set; } = true;
    
    /// <summary>
    /// The seed color to use when a dynamic seed color is not available or dynamic theming is disabled.
    /// </summary>
    public uint FallbackSeed { get; set; } = 0xff4285F4; // Google Blue

    /// <summary>
    /// Whether to use wallpaper/accent color based dynamic theming. Defaults to true.
    /// </summary>
    /// <remarks>
    /// When set to <see langword="false"/>, <see cref="FallbackSeed"/> will be used as seed,
    /// even on platforms that expose a dynamic color source.
    /// </remarks>
    public bool EnableDynamicColor { get; set; } = true;

    /// <summary>
    /// The style used by default to create the core palette.
    /// </summary>
    /// <remarks>
    /// Can be updated at runtime using MaterialColorService.Style.
    /// </remarks>
    public Style DefaultStyle { get; set; } = Style.TonalSpot;
}