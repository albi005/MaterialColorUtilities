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

namespace MaterialColorUtilities.ColorAppearance;

public class Hct
{
    private double hue;
    private double chroma;
    private double tone;
    private int argb;

    public static Hct From(double hue, double chroma, double tone)
    {
        int argb = CamSolver.SolveToInt(hue, chroma, tone);
        return new(argb);
    }

    public static Hct FromInt(int argb) => new(argb);

    private Hct(int argb)
    {
        SetInternalState(argb);
    }

    public double Hue
    {
        get => hue;
        set => SetInternalState(CamSolver.SolveToInt(value, chroma, tone));
    }
    public double Chroma
    {
        get => chroma;
        set => SetInternalState(CamSolver.SolveToInt(hue, value, tone));
    }
    public double Tone
    {
        get => tone;
        set => SetInternalState(CamSolver.SolveToInt(hue, chroma, value));
    }

    public int ToInt() => argb;

    private void SetInternalState(int argb)
    {
        this.argb = argb;
        Cam16 cam = Cam16.FromInt(argb);
        hue = cam.Hue;
        chroma = cam.Chroma;
        tone = ColorUtils.LStarFromArgb(argb);
    }
}
