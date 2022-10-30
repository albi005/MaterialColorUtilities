namespace MaterialColorUtilities.Palettes;

/// <summary>
/// Decides what hue and chroma the different tonal palettes should have.
/// </summary>
public enum Style
{
    Spritz,
        
    /// <summary>
    /// The default style. Use when theming based on the user's wallpaper.
    /// More on <seealso href="https://m3.material.io/styles/color/dynamic-color/user-generated-color#35bc06c5-35d9-4559-9f5d-07ea734cbcb1">m3.material.io</seealso>
    /// </summary>
    TonalSpot,
    
    Vibrant,
    
    Expressive,
    
    Rainbow,
    
    FruitSalad,

    /// <summary>
    /// Use when theming based on in-app content.
    /// More on <seealso href="https://m3.material.io/styles/color/dynamic-color/user-generated-color#8af550b9-a19e-4e9f-bb0a-7f611fed5d0f">m3.material.io</seealso>
    /// </summary>
    Content
}