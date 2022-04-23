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

public class TonalPalette
{
    private readonly Dictionary<int, int> cache = new();
    private readonly double hue;
    private readonly double chroma;

    /**
     * Create tones using the HCT hue and chroma from a color.
     *
     * @param argb ARGB representation of a color
     * @return Tones matching that color's hue and chroma.
     */
    public static TonalPalette FromInt(int argb)
    {
        Hct hct = Hct.FromInt(argb);
        return FromHueAndChroma(hct.Hue, hct.Chroma);
    }

    /**
     * Create tones from a defined HCT hue and chroma.
     *
     * @param hue HCT hue
     * @param chroma HCT chroma
     * @return Tones matching hue and chroma.
     */
    public static TonalPalette FromHueAndChroma(double hue, double chroma) => new(hue, chroma);

    private TonalPalette(double hue, double chroma)
    {
        this.hue = hue;
        this.chroma = chroma;
    }

    /**
     * Create an ARGB color with HCT hue and chroma of this Tones instance, and the provided HCT tone.
     *
     * @param tone HCT tone, measured from 0 to 100.
     * @return ARGB representation of a color with that tone.
     */
    public int Tone(int tone)
        => cache.TryGetValue(tone, out int value)
            ? value
            : cache[tone] = Hct.From(hue, chroma, tone).ToInt();

    public int this[int tone] => Tone(tone);
}
