namespace MaterialColorUtilities.Maui;

public sealed class DynamicColorOptions
{
    /// <summary>
    /// When true, updates to Application.UserAppTheme will be stored using Preferences
    /// and reapplied when the app restarts. Defaults to true.
    /// </summary>
    public bool RememberIsDark { get; set; } = true;

    /// <summary>
    /// Determines whether to generate colors as application resources.
    /// Set to false when using a custom color scheme source.
    /// </summary>
    /// <remarks>
    /// Can be updated at runtime using DynamicColorService.IsEnabled.
    /// </remarks>
    public bool EnableTheming { get; set; } = true;
    
    /// <summary>
    /// Will be used if a dynamic accent color is not available or dynamic theming is disabled.
    /// </summary>
    public int FallbackSeed { get; set; } = unchecked((int)0xff4285F4); // Google Blue

    /// <summary>
    /// Whether to use wallpaper/accent color based dynamic theming. Defaults to true.
    /// </summary>
    /// <remarks>
    /// When set to <see langword="false"/>, <see cref="FallbackSeed"/> will be used as seed,
    /// even on platforms that expose an accent color.
    /// </remarks>
    public bool EnableDynamicColor { get; set; } = true;
}