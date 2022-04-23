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
/// An intermediate concept between the key color for a UI theme, and a full color scheme. 5 sets of
/// tones are generated, all except one use the same hue as the key color, and all vary in chroma.
/// </summary>
public class CorePalette
{
    public TonalPalette Primary { get; set; }
    public TonalPalette Secondary { get; set; }
    public TonalPalette Tertiary { get; set; }
    public TonalPalette Neutral { get; set; }
    public TonalPalette NeutralVariant { get; set; }
    public TonalPalette Error { get; set; }

    /// <summary>Create key tones from a source ARGB color.</summary>
    /// <param name="argb">ARGB representation of a color.</param>
    public static CorePalette Of(int seed) => new(seed);

    /// <summary>Create key tones from a source ARGB color.</summary>
    /// <param name="argb">ARGB representation of a color.</param>
    public CorePalette(int seed)
    {
        Hct hct = Hct.FromInt(seed);
        double hue = hct.Hue;
        Primary = TonalPalette.FromHueAndChroma(hue, Math.Max(48, hct.Chroma));
        Secondary = TonalPalette.FromHueAndChroma(hue, 16);
        Tertiary = TonalPalette.FromHueAndChroma(hue + 60, 24);
        Neutral = TonalPalette.FromHueAndChroma(hue, 4);
        NeutralVariant = TonalPalette.FromHueAndChroma(hue, 8);
        Error = TonalPalette.FromHueAndChroma(25, 84);
    }
}
