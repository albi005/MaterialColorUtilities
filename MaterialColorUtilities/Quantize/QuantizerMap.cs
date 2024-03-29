﻿// Copyright 2021 Google LLC
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

using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Quantize;

/// <summary>
/// Creates a dictionary with keys of colors, and values of count of the color.
/// </summary>
public class QuantizerMap : IQuantizer
{
    public Dictionary<uint, uint> ColorToCount { get; } = new();

    public QuantizerResult Quantize(uint[] pixels, uint colorCount)
    {
        foreach (uint pixel in pixels)
        {
            uint alpha = ColorUtils.AlphaFromArgb(pixel);
            if (alpha < 255)
                continue;
            if (ColorToCount.ContainsKey(pixel))
                ColorToCount[pixel]++;
            else
                ColorToCount[pixel] = 1;
        }
        return new(ColorToCount);
    }
}
