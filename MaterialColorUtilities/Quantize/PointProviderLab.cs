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

using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Quantize;

public class PointProviderLab : IPointProvider
{
    public double[] FromInt(int argb)
    {
        double[] lab = ColorUtils.LabFromArgb(argb);
        return new double[] { lab[0], lab[1], lab[2] };
    }

    public int ToInt(double[] lab)
    {
        return ColorUtils.ArgbFromLab(lab[0], lab[1], lab[2]);
    }

    public double Distance(double[] one, double[] two)
    {
        double dL = one[0] - two[0];
        double dA = one[1] - two[1];
        double dB = one[2] - two[2];
        return dL * dL + dA * dA + dB * dB;
    }
}
