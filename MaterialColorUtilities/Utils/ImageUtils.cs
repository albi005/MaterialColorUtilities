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

using MaterialColorUtilities.Quantize;
using MaterialColorUtilities.Score;

namespace MaterialColorUtilities.Utils;

public static class ImageUtils
{
    /// <summary>
    /// Extracts colors from an image.
    /// </summary>
    /// <param name="pixels">The colors of the image in ARGB format.</param>
    /// <returns>
    /// The extracted colors in descending order by score.
    /// At least one color will be returned.
    /// </returns>
    public static List<int> ColorsFromImage(int[] pixels)
    {
        var result = QuantizerCelebi.Quantize(pixels, 128);
        var ranked = Scorer.Score(result);
        return ranked;
    }
}
