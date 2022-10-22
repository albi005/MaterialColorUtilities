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
/// A convenience class for retrieving colors that are constant in hue and
/// chroma, but vary in tone.
/// </summary>
public class TonalPalette
{
    private readonly Dictionary<uint, uint> cache = new();
    private readonly double hue;
    private readonly double chroma;

    /// <summary>Creates tones using the HCT hue and chroma from a color.</summary>
    /// <param name="argb">ARGB representation of a color.</param>
    /// <returns>Tones matching that color's hue and chroma.</returns>
    public static TonalPalette FromInt(uint argb)
    {
        Hct hct = Hct.FromInt(argb);
        return FromHueAndChroma(hct.Hue, hct.Chroma);
    }

    /// <summary>Creates tones from a defined HCT hue and chroma.</summary>
    /// <param name="hue">HCT hue</param>
    /// <param name="chroma">HCT chroma</param>
    /// <returns>Tones matching hue and chroma.</returns>
    public static TonalPalette FromHueAndChroma(double hue, double chroma) => new(hue, chroma);

    private TonalPalette(double hue, double chroma)
    {
        this.hue = hue;
        this.chroma = chroma;
    }

    /// <summary>Creates an ARGB color with HCT hue and chroma of this TonalPalette instance, and the provided HCT tone.</summary>
    /// <param name="tone">HCT tone, measured from 0 to 100.</param>
    /// <returns>ARGB representation of a color with that tone.</returns>
    public uint Tone(uint tone)
        => cache.TryGetValue(tone, out uint value)
            ? value
            : cache[tone] = Hct.From(hue, chroma, tone).ToInt();

    /// <summary>Creates an ARGB color with HCT hue and chroma of this TonalPalette instance, and the provided HCT tone.</summary>
    /// <param name="tone">HCT tone, measured from 0 to 100.</param>
    /// <returns>ARGB representation of a color with that tone.</returns>
    public uint this[uint tone] => Tone(tone);
}
