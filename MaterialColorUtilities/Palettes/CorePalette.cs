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
using MaterialColorUtilities.Utils;

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
    public static CorePalette Of(uint seed) => Of(seed, Style.TonalSpot);

    /// <summary>Create content key tones from a color.</summary>
    /// <param name="seed">ARGB representation of a color.</param>
    public static CorePalette ContentOf(uint seed) => Of(seed, Style.Content);

    /// <summary>Create key tones from a color.</summary>
    /// <param name="seed">ARGB representation of a color.</param>
    /// <param name="style">
    /// The strategy that decides what hue and chroma the created tonal palettes should have.
    /// </param>
    public static CorePalette Of(uint seed, Style style)
    {
        CorePalette corePalette = new();
        corePalette.Fill(seed, style);
        return corePalette;
    }

    public virtual void Fill(uint seed, Style style = Style.TonalSpot)
    {
        Hct hct = Hct.FromInt(seed);
        double hue = hct.Hue;
        double chroma = hct.Chroma;
        
        // From https://android.googlesource.com/platform/frameworks/base/+/5ecdfa15559482676402d61463cc51faeb6e18c8/packages/SystemUI/monet/src/com/android/systemui/monet/ColorScheme.kt#158
        switch (style)
        {
            case Style.Spritz:
                Primary = new(hue, 12);
                Secondary = new(hue, 8);
                Tertiary = new(hue, 16);
                Neutral = new(hue, 2);
                NeutralVariant = new(hue, 2);
                break;
            case Style.TonalSpot:
                Primary = new(hue, 36);
                Secondary = new(hue, 16);
                Tertiary = new(hue + 60, 24);
                Neutral = new(hue, 4);
                NeutralVariant = new(hue, 8);
                break;
            case Style.Vibrant:
                Primary = new(hue, 130);
                Secondary = new(
                    MathUtils.RotateHue(hue, (0, 18), (41, 15), (61, 10), (101, 12), (131, 15), (181, 18), (251, 15), (301, 12), (360, 12)),
                    24);
                Tertiary = new(
                    MathUtils.RotateHue(hue, (0, 35), (41, 30), (61, 20), (101, 25), (131, 30), (181, 35), (251, 30), (301, 25), (360, 25)),
                    32);
                Neutral = new(hue, 10);
                NeutralVariant = new(hue, 12);
                break;
            case Style.Expressive:
                Primary = new(hue + 240, 40);
                Secondary = new(
                    MathUtils.RotateHue(hue, (0, 45), (21, 95), (51, 45), (121, 20), (151, 45), (191, 90), (271, 45), (321, 45), (360, 45)),
                    24);
                Tertiary = new(
                    MathUtils.RotateHue(hue, (0, 120), (21, 120), (51, 20), (121, 45), (151, 20), (191, 15), (271, 20), (321, 120), (360, 120)),
                    32);
                Neutral = new(hue + 15, 8);
                NeutralVariant = new(hue + 15, 12);
                break;
            case Style.Rainbow:
                Primary = new(hue, 48);
                Secondary = new(hue, 16);
                Tertiary = new(hue + 60, 24);
                Neutral = new(hue, 0);
                NeutralVariant = new(hue, 0);
                break;
            case Style.FruitSalad:
                Primary = new(hue - 50, 48);
                Secondary = new(hue - 50, 36);
                Tertiary = new(hue, 36);
                Neutral = new(hue, 10);
                NeutralVariant = new(hue, 16);
                break;
            case Style.Content:
                Primary = new(hue, chroma);
                Secondary = new(hue, chroma * 0.33);
                Tertiary = new(hue, chroma * 0.66);
                Neutral = new(hue, chroma * 0.0833);
                NeutralVariant = new(hue, chroma * 0.1666);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }
        Error = new(25, 84);
    }
}
