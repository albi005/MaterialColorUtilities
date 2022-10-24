// Copyright 2021 Google LLC
// Copyright 2021-2022 project contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using MaterialColorUtilities.ColorAppearance;

namespace MaterialColorUtilities.Palettes;

/// <summary>
/// Contains tonal palettes for the key colors.
/// </summary>
public class CorePalette
{
    public TonalPalette Primary { get; set; } = null!;
    public TonalPalette Secondary { get; set; } = null!;
    public TonalPalette Tertiary { get; set; } = null!;
    public TonalPalette Neutral { get; set; } = null!;
    public TonalPalette NeutralVariant { get; set; } = null!;
    public TonalPalette Error { get; set; } = null!;

    /// <summary>Creates an empty core palette.</summary>
    public CorePalette()
    {
    }

    /// <summary>Create key tones from a color using the default strategy.</summary>
    /// <param name="seed">ARGB representation of a color.</param>
    public static CorePalette Of(uint seed) => Of(seed, Strategy.Default);

    /// <summary>Create content key tones from a color.</summary>
    /// <param name="seed">ARGB representation of a color.</param>
    public static CorePalette ContentOf(uint seed) => Of(seed, Strategy.Content);

    /// <summary>Create key tones from a color.</summary>
    /// <param name="seed">ARGB representation of a color.</param>
    /// <param name="strategy">
    /// The strategy that decides what hue and chroma the created tonal palettes should have.
    /// </param>
    public static CorePalette Of(uint seed, Strategy strategy)
    {
        CorePalette corePalette = new();
        corePalette.Fill(seed, strategy);
        return corePalette;
    }

    public virtual void Fill(uint seed, Strategy strategy = Strategy.Default)
    {
        Hct hct = Hct.FromInt(seed);
        double hue = hct.Hue;
        double chroma = hct.Chroma;
        
        switch (strategy)
        {
            case Strategy.Default:
                Primary = TonalPalette.FromHueAndChroma(hue, Math.Max(48, chroma));
                Secondary = TonalPalette.FromHueAndChroma(hue, 16);
                Tertiary = TonalPalette.FromHueAndChroma(hue + 60, 24);
                Neutral = TonalPalette.FromHueAndChroma(hue, 4);
                NeutralVariant = TonalPalette.FromHueAndChroma(hue, 8);
                break;
            case Strategy.Content:
                Primary = TonalPalette.FromHueAndChroma(hue, chroma);
                Secondary = TonalPalette.FromHueAndChroma(hue, chroma / 3);
                Tertiary = TonalPalette.FromHueAndChroma(hue + 60, chroma / 2);
                Neutral = TonalPalette.FromHueAndChroma(hue, Math.Min(chroma / 12, 4));
                NeutralVariant = TonalPalette.FromHueAndChroma(hue, Math.Min(chroma / 6, 8));
                break;
            case Strategy.A:
                Primary = TonalPalette.FromHueAndChroma(hue, 12);
                Secondary = TonalPalette.FromHueAndChroma(hue, 8);
                Tertiary = TonalPalette.FromHueAndChroma(hue, 16);
                Neutral = TonalPalette.FromHueAndChroma(hue, 2);
                NeutralVariant = TonalPalette.FromHueAndChroma(hue, 2);
                break;
            case Strategy.B:
                Primary = TonalPalette.FromHueAndChroma(hue, 150);
                Secondary = TonalPalette.FromHueAndChroma(hue + 15, 24);
                Tertiary = TonalPalette.FromHueAndChroma(hue + 30, 32);
                Neutral = TonalPalette.FromHueAndChroma(hue, 10);
                NeutralVariant = TonalPalette.FromHueAndChroma(hue, 12);
                break;
            case Strategy.C:
                Primary = TonalPalette.FromHueAndChroma(hue - 120, 40);
                Secondary = TonalPalette.FromHueAndChroma(hue switch
                {
                    > 30 and < 50 => hue + 95,
                    > 130 and < 150 => hue + 20,
                    > 200 and < 265 => hue + 90,
                    _ => hue + 45
                }, 24);
                Tertiary = TonalPalette.FromHueAndChroma(hue + 135, 32);
                Neutral = TonalPalette.FromHueAndChroma(hue + 15, 8);
                NeutralVariant = TonalPalette.FromHueAndChroma(hue + 15, 12);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
        }
        Error = TonalPalette.FromHueAndChroma(25, 84);
    }

    /// <summary>
    /// Decides what hue and chroma the different tonal palettes should have.
    /// </summary>
    public enum Strategy
    {
        /// <summary>
        /// The default strategy. Use when theming based on the user's wallpaper.
        /// </summary>
        /// <remarks>
        /// All tonal palettes except tertiary use the same
        /// hue as the seed color, and all vary in chroma. <br/>
        /// More on <a href="https://m3.material.io/styles/color/dynamic-color/user-generated-color#35bc06c5-35d9-4559-9f5d-07ea734cbcb1">m3.material.io</a>
        /// </remarks>
        Default,
        
        /// <summary>
        /// Use when theming based on in-app content.
        /// </summary>
        /// <remarks>
        /// More on <a href="https://m3.material.io/styles/color/dynamic-color/user-generated-color#8af550b9-a19e-4e9f-bb0a-7f611fed5d0f">m3.material.io</a>
        /// </remarks>
        Content,
        
        /// <summary>
        /// All of the tonal palettes use the same hue as the seed and with low chroma.
        /// </summary>
        /// <remarks>
        /// Approximation of the 2. option available on Google Pixel phones.
        /// </remarks>
        A,
        
        /// <summary>
        /// All of the tonal palettes use the same hue as the seed other than secondary and tertiary.
        /// Secondary is 15 higher and tertiary is 30 higher.
        /// They all have relatively high chroma.
        /// </summary>
        /// <remarks>
        /// Approximation of the 3. option available on Google Pixel phones.
        /// </remarks>
        B,
        
        /// <summary>
        /// Produces vibrant palettes using a diverse set of hues. 
        /// </summary>
        /// <remarks>
        /// Approximation of the 4. option available on Google Pixel phones.
        /// </remarks>
        C
    }
}
