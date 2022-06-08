// Copyright(c) 2021 Bjorn Ottosson
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files(the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// From https://github.com/behreajj/UnityOkHsl/blob/93d5c86a9e107f53dc72d951cce624f8404f0ba1/OkColor.cs

namespace Playground.Wasm;

public static class OkColor
{

    // Finds the maximum saturation possible for a given hue that fits in sRGB
    // Saturation here is defined as S = C/L
    // a and b must be normalized so a^2 + b^2 == 1
    private static double ComputeMaxSaturation(in double a, in double b)
    {
        // Max saturation will be when one of r, g or b goes below zero.

        // Select different coefficients depending on which component goes below zero first
        double k0, k1, k2, k3, k4, wl, wm, ws;

        if (-1.88170328d * a - 0.80936493d * b > 1.0d)
        {
            // Re component
            k0 = 1.19086277d;
            k1 = 1.76576728d;
            k2 = 0.59662641d;
            k3 = 0.75515197d;
            k4 = 0.56771245d;
            wl = 4.0767416621d;
            wm = -3.3077115913d;
            ws = 0.2309699292d;
        }
        else if (1.81444104d * a - 1.19445276d * b > 1.0d)
        {
            // Green component
            k0 = 0.73956515d;
            k1 = -0.45954404d;
            k2 = 0.08285427d;
            k3 = 0.1254107d;
            k4 = 0.14503204d;
            wl = -1.2684380046d;
            wm = 2.6097574011d;
            ws = -0.3413193965d;
        }
        else
        {
            // Blue component
            k0 = 1.35733652d;
            k1 = -0.00915799d;
            k2 = -1.1513021d;
            k3 = -0.50559606d;
            k4 = +0.00692167d;
            wl = -0.0041960863d;
            wm = -0.7034186147d;
            ws = 1.707614701d;
        }

        // Approximate max saturation using a polynomial:
        double S = k0 + k1 * a + k2 * b + k3 * a * a + k4 * a * b;

        // Do one step Halley's method to get closer
        // this gives an error less than 10e6, except for some blue hues where the dS/dh is close to infinite
        // this should be sufficient for most applications, otherwise do two/three steps 

        double k_l = 0.3963377774d * a + 0.2158037573d * b;
        double k_m = -0.1055613458d * a - 0.0638541728d * b;
        double k_s = -0.0894841775d * a - 1.291485548d * b;

        {
            double l_ = 1.0d + S * k_l;
            double m_ = 1.0d + S * k_m;
            double s_ = 1.0d + S * k_s;

            double l = l_ * l_ * l_;
            double m = m_ * m_ * m_;
            double s = s_ * s_ * s_;

            double l_dS = 3.0d * k_l * l_ * l_;
            double m_dS = 3.0d * k_m * m_ * m_;
            double s_dS = 3.0d * k_s * s_ * s_;

            double l_dS2 = 6.0d * k_l * k_l * l_;
            double m_dS2 = 6.0d * k_m * k_m * m_;
            double s_dS2 = 6.0d * k_s * k_s * s_;

            double f = wl * l + wm * m + ws * s;
            double f1 = wl * l_dS + wm * m_dS + ws * s_dS;
            double f2 = wl * l_dS2 + wm * m_dS2 + ws * s_dS2;

            double sDenom = f1 * f1 - 0.5d * f * f2;
            if (sDenom != 0.0d) { S = S - f * f1 / sDenom; }
        }

        return S;
    }

    private static (double L, double C) FindCusp(in double a, in double b)
    {
        // First, find the maximum saturation (saturation S = C/L)
        double S_cusp = ComputeMaxSaturation(a, b);

        // Convert to linear sRGB to find the first point where at least one of r,g or b >= 1:
        var rgb_at_max = OkLabToLinearSrgb((L: 1.0d, a: S_cusp * a, b: S_cusp * b));
        double L_cusp = Math.Pow(1.0d / Math.Max(Math.Max(rgb_at_max.r, rgb_at_max.g), rgb_at_max.b), 1.0d / 3.0d);
        double C_cusp = L_cusp * S_cusp;

        return (L: L_cusp, C: C_cusp);
    }

    // Finds intersection of the line defined by 
    // L = L0 * (1 - t) + t * L1;
    // C = t * C1;
    // a and b must be normalized so a^2 + b^2 == 1
    private static double FindGamutIntersection(in double a, in double b, in double L1, in double C1, in double L0, (double L, double C) cusp)
    {
        // Find the intersection for upper and lower half seprately
        double t = 0.0d;
        if ((L1 - L0) * cusp.C - (cusp.L - L0) * C1 <= 0.0d)
        {
            // Lower half
            double tDenom = C1 * cusp.L + cusp.C * (L0 - L1);
            if (tDenom != 0.0d) { t = cusp.C * L0 / tDenom; }
        }
        else
        {
            // Upper half

            // First intersect with triangle
            double tDenom = C1 * (cusp.L - 1.0d) + cusp.C * (L0 - L1);
            if (tDenom != 0.0d) { t = cusp.C * (L0 - 1.0d) / tDenom; }

            // Then one step Halley's method
            {
                double dL = L1 - L0;
                double dC = C1;

                double k_l = 0.3963377774d * a + 0.2158037573d * b;
                double k_m = -0.1055613458d * a - 0.0638541728d * b;
                double k_s = -0.0894841775d * a - 1.2914855480d * b;

                double l_dt = dL + dC * k_l;
                double m_dt = dL + dC * k_m;
                double s_dt = dL + dC * k_s;

                // If higher accuracy is required, 2 or 3 iterations of the following block can be used:
                {
                    double L = L0 * (1.0d - t) + t * L1;
                    double C = t * C1;

                    double l_ = L + C * k_l;
                    double m_ = L + C * k_m;
                    double s_ = L + C * k_s;

                    double l = l_ * l_ * l_;
                    double m = m_ * m_ * m_;
                    double s = s_ * s_ * s_;

                    double ldt = 3.0d * l_dt * l_ * l_;
                    double mdt = 3.0d * m_dt * m_ * m_;
                    double sdt = 3.0d * s_dt * s_ * s_;

                    double ldt2 = 6.0d * l_dt * l_dt * l_;
                    double mdt2 = 6.0d * m_dt * m_dt * m_;
                    double sdt2 = 6.0d * s_dt * s_dt * s_;

                    double r0 = 4.0767416621d * l - 3.3077115913d * m + 0.2309699292d * s - 1.0d;
                    double r1 = 4.0767416621d * ldt - 3.3077115913d * mdt + 0.2309699292d * sdt;
                    double r2 = 4.0767416621d * ldt2 - 3.3077115913d * mdt2 + 0.2309699292d * sdt2;

                    double rDenom = r1 * r1 - 0.5d * r0 * r2;
                    double ur = rDenom != 0.0d ? r1 / rDenom : 0.0d;
                    double tr = -r0 * ur;

                    double g0 = -1.2684380046d * l + 2.6097574011d * m - 0.3413193965d * s - 1.0d;
                    double g1 = -1.2684380046d * ldt + 2.6097574011d * mdt - 0.3413193965d * sdt;
                    double g2 = -1.2684380046d * ldt2 + 2.6097574011d * mdt2 - 0.3413193965d * sdt2;

                    double gDenom = g1 * g1 - 0.5d * g0 * g2;
                    double ug = gDenom != 0.0d ? g1 / gDenom : 0.0d;
                    double tg = -g0 * ug;

                    double b0 = -0.0041960863d * l - 0.7034186147d * m + 1.7076147010d * s - 1.0d;
                    double b1 = -0.0041960863d * ldt - 0.7034186147d * mdt + 1.7076147010d * sdt;
                    double b2 = -0.0041960863d * ldt2 - 0.7034186147d * mdt2 + 1.7076147010d * sdt2;

                    double bDenom = b1 * b1 - 0.5d * b0 * b2;
                    double ub = bDenom != 0.0d ? b1 / bDenom : 0.0d;
                    double tb = -b0 * ub;

                    tr = ur >= 0.0d ? tr : float.MaxValue;
                    tg = ug >= 0.0d ? tg : float.MaxValue;
                    tb = ub >= 0.0d ? tb : float.MaxValue;

                    t = t + Math.Min(tr, Math.Min(tg, tb));
                }
            }
        }

        return t;
    }

    private static (double C_0, double C_mid, double C_max) GetCs(in double L, in double a_, in double b_)
    {
        var cusp = FindCusp(a_, b_);

        double C_max = FindGamutIntersection(a_, b_, L, 1.0d, L, cusp);
        var ST_max = ToSt(cusp);

        // Scale factor to compensate for the curved part of gamut shape:
        double k = C_max / Math.Min(L * ST_max.S, (1.0d - L) * ST_max.T);

        double C_mid = 0.0d;
        {
            var ST_mid = GetSTMid(a_, b_);

            // Use a soft minimum function, instead of a sharp triangle shape to get a smooth value for chroma.
            double C_a = L * ST_mid.S;
            double C_b = (1.0d - L) * ST_mid.T;
            double cae4 = C_a * C_a * C_a * C_a;
            double cbe4 = C_b * C_b * C_b * C_b;
            C_mid = 0.9d * k * Math.Sqrt(Math.Sqrt(1.0d / (1.0d / cae4 + 1.0d / cbe4)));
        }

        double C_0 = 0.0d;
        {
            // for C_0, the shape is independent of hue, so ST are constant. Values picked to roughly be the average values of ST.
            double C_a = L * 0.4d;
            double C_b = (1.0d - L) * 0.8d;

            // Use a soft minimum function, instead of a sharp triangle shape to get a smooth value for chroma.
            C_0 = Math.Sqrt(1.0d / (1.0d / (C_a * C_a) + 1.0d / (C_b * C_b)));
        }

        return (C_0, C_mid, C_max);
    }

    // Returns a smooth approximation of the location of the cusp
    // This polynomial was created by an optimization process
    // It has been designed so that S_mid < S_max and T_mid < T_max
    private static (double S, double T) GetSTMid(in double a_, in double b_)
    {
        double S = 0.11516993d;
        double sDenom = 7.4477897d + 4.1590124d * b_ +
            a_ * (-2.19557347d + 1.75198401d * b_ +
                a_ * (-2.13704948d - 10.02301043d * b_ +
                    a_ * (-4.24894561d + 5.38770819d * b_ +
                        4.69891013d * a_)));
        if (sDenom != 0.0d) { S += 1.0d / sDenom; }

        double T = 0.11239642d;
        double tDenom = 1.6132032d - 0.68124379d * b_ +
            a_ * (0.40370612d + 0.90148123d * b_ +
                a_ * (-0.27087943d + 0.6122399d * b_ +
                    a_ * (0.00299215d - 0.45399568d * b_ - 0.14661872d * a_)));
        if (tDenom != 0.0d) { T += 1.0d / tDenom; }

        return (S, T);
    }

    private static (double L, double a, double b) LinearSrgbToOkLab(in (double r, double g, double b) c)
    {
        double cr = c.r;
        double cg = c.g;
        double cb = c.b;

        double l = 0.4122214708d * cr + 0.5363325363d * cg + 0.0514459929d * cb;
        double m = 0.2119034982d * cr + 0.6806995451d * cg + 0.1073969566d * cb;
        double s = 0.0883024619d * cr + 0.2817188376d * cg + 0.6299787005d * cb;

        double lCbrt = Math.Pow(l, 1.0d / 3.0d);
        double mCbrt = Math.Pow(m, 1.0d / 3.0d);
        double sCbrt = Math.Pow(s, 1.0d / 3.0d);

        return (
            L: 0.2104542553d * lCbrt + 0.793617785d * mCbrt - 0.0040720468d * sCbrt,
            a: 1.9779984951d * lCbrt - 2.428592205d * mCbrt + 0.4505937099d * sCbrt,
            b: 0.0259040371d * lCbrt + 0.7827717662d * mCbrt - 0.808675766d * sCbrt);
    }

    public static (double L, double a, double b) OkHslToOkLab(in (double h, double s, double l) hsl)
    {
        // With single-precision numbers, this can generate invalid values, NaNs, infinities, etc.

        double l = hsl.l;
        if (l >= 1.0d) { return (L: 1.0d, a: 0.0d, b: 0.0d); }
        if (l <= 0.0d) { return (L: 0.0d, a: 0.0d, b: 0.0d); }

        double s = hsl.s;
        if (s < 0.0d) { s = 0.0d; }
        if (s > 1.0d) { s = 1.0d; }

        double hRad = hsl.h * 6.283185307179586d;
        double a_ = Math.Cos(hRad);
        double b_ = Math.Sin(hRad);
        double L = ToeInv(l);

        var cs = GetCs(L, a_, b_);
        double c0 = cs.C_0;
        double cMid = cs.C_mid;
        double cMax = cs.C_max;

        double mid = 0.8d;
        double midInv = 1.25d;

        double C = 0.0d;
        double t = 0.0d;
        double k0 = 0.0d;
        double k1 = 0.0d;
        double k2 = 1.0d;

        if (s < mid)
        {
            t = midInv * s;

            k1 = mid * c0;
            if (cMid != 0.0d) { k2 = 1.0d - k1 / cMid; }

            double kDenom = 1.0d - k2 * t;
            if (kDenom != 0.0d) { C = t * k1 / kDenom; }
        }
        else
        {
            double tDenom = 1.0d - mid;
            if (tDenom != 0.0d) { t = (s - mid) / tDenom; }

            k0 = cMid;
            if (c0 != 0.0d) { k1 = (1.0d - mid) * cMid * cMid * midInv * midInv / c0; }

            double cDenom = cMax - cMid;
            k2 = 1.0d;
            if (cDenom != 0.0d) { k2 = 1.0d - k1 / cDenom; }

            double kDenom = 1.0d - k2 * t;
            if (kDenom != 0.0d) { C = k0 + t * k1 / kDenom; }
        }

        return (L, a: C * a_, b: C * b_);
    }

    public static (double r, double g, double b) OkHslToSrgb(in (double h, double s, double l) hsl)
    {
        return OkLabToSrgb(OkHslToOkLab(hsl));
    }

    public static (double L, double a, double b) OkHsvToOkLab(in (double h, double s, double v) hsv)
    {
        double v = hsv.v;
        if (v <= 0.0d) { return (L: 0.0d, a: 0.0d, b: 0.0d); }
        if (v > 1.0d) { v = 1.0d; }

        double s = hsv.s;
        if (s < 0.0d) { s = 0.0d; }
        if (s > 1.0d) { s = 1.0d; }

        double hRad = hsv.h * 6.283185307179586d;
        double a_ = Math.Cos(hRad);
        double b_ = Math.Sin(hRad);

        var cusp = FindCusp(a_, b_);
        var stMax = ToSt(cusp);
        double sMax = stMax.S;
        double tMax = stMax.T;
        double s0 = 0.5d;
        double k = 1.0d;
        if (sMax != 0.0d) { k = 1.0d - s0 / sMax; }

        // first we compute L and V as if the gamut is a perfect triangle:

        // L, C when v==1:
        double vDenom = s0 + tMax - tMax * k * s;
        double lv = 1.0d;
        double cv = 0.0d;
        if (vDenom != 0.0d)
        {
            lv = 1.0d - s * s0 / vDenom;
            cv = s * tMax * s0 / vDenom;
        }

        double L = v * lv;
        double C = v * cv;

        // then we compensate for both toe and the curved top part of the triangle:
        double lvt = ToeInv(lv);
        double cvt = 0.0d;
        if (lv != 0.0d) { cvt = cv * lvt / lv; }

        double lNew = ToeInv(L);
        if (L != 0.0d) { C = C * lNew / L; }
        L = lNew;

        var rgbScale = OkLabToLinearSrgb((L: lvt, a: a_ * cvt, b: b_ * cvt));
        double maxComp = Math.Max(rgbScale.r, Math.Max(rgbScale.g, Math.Max(rgbScale.b, 0.0d)));
        double lScale = 0.0d;
        if (maxComp != 0.0d)
        {
            lScale = Math.Pow(1.0d / maxComp, 1.0d / 3.0d);
        }

        C = C * lScale;
        return (
            L: L * lScale,
            a: C * a_,
            b: C * b_);
    }

    public static (double r, double g, double b) OkHsvToSrgb(in (double h, double s, double v) hsv)
    {
        return OkLabToSrgb(OkHsvToOkLab(hsv));
    }

    static (double r, double g, double b) OkLabToLinearSrgb(in (double L, double a, double b) c)
    {
        double cl = c.L;
        double ca = c.a;
        double cb = c.b;

        double lCbrt = cl + 0.3963377774d * ca + 0.2158037573d * cb;
        double mCbrt = cl - 0.1055613458d * ca - 0.0638541728d * cb;
        double sCbrt = cl - 0.0894841775d * ca - 1.291485548d * cb;

        double l = lCbrt * lCbrt * lCbrt;
        double m = mCbrt * mCbrt * mCbrt;
        double s = sCbrt * sCbrt * sCbrt;

        return (
            r: 4.0767416621d * l - 3.3077115913d * m + 0.2309699292d * s,
            g: -1.2684380046d * l + 2.6097574011d * m - 0.3413193965d * s,
            b: -0.0041960863d * l - 0.7034186147d * m + 1.707614701d * s);
    }

    public static (double h, double s, double l) OkLabToOkHsl(in (double L, double a, double b) lab)
    {
        double L = lab.L;
        if (L > 1.0d - float.Epsilon) { return (h: 0.0d, s: 0.0d, l: 1.0d); }
        if (L < float.Epsilon) { return (h: 0.0d, s: 0.0d, l: 0.0d); }

        // This has to be gt epsilon, not gt zero to avoid glitches.
        double Csq = lab.a * lab.a + lab.b * lab.b;
        if (Csq > float.Epsilon)
        {
            double C = Math.Sqrt(Csq);
            double a_ = lab.a / C;
            double b_ = lab.b / C;

            // 1.0 / math.pi = 0.3183098861837907
            double h = 0.5d + 0.5d * (Math.Atan2(-lab.b, -lab.a) * 0.3183098861837907d);

            var cs = GetCs(L, a_, b_);
            double c0 = cs.C_0;
            double cMid = cs.C_mid;
            double cMax = cs.C_max;

            // Inverse of the interpolation in okhsl_to_srgb:
            double mid = 0.8d;
            double midInv = 1.25d;

            double s = 0.0d;
            if (C < cMid)
            {
                double k1 = mid * c0;
                double k2 = 1.0d;
                if (cMid != 0.0d) { k2 = 1.0d - k1 / cMid; }

                double tDenom = k1 + k2 * C;
                double t = 0.0d;
                if (tDenom != 0.0d) { t = C / tDenom; }

                s = t * mid;
            }
            else
            {
                double k0 = cMid;
                double k1 = 0.0d;
                if (c0 != 0.0d)
                {
                    k1 = (1.0d - mid) * cMid * cMid * midInv * midInv / c0;
                }

                double cDenom = cMax - cMid;
                double k2 = 1.0d;
                if (cDenom != 0.0d) { k2 = 1.0d - k1 / cDenom; }

                double tDenom = k1 + k2 * (C - k0);
                double t = 0.0d;
                if (tDenom != 0.0d) { t = (C - k0) / tDenom; }

                s = mid + (1.0d - mid) * t;
            }

            return (h, s, l: Toe(L));
        }
        else
        {
            return (h: 0.0d, s: 0.0d, l: L);
        }
    }

    public static (double h, double s, double v) OkLabToOkHsv(in (double L, double a, double b) lab)
    {
        double L = lab.L;
        if (L > 1.0d - float.Epsilon) { return (h: 0.0d, s: 0.0d, v: 1.0d); }
        if (L < float.Epsilon) { return (h: 0.0d, s: 0.0d, v: 0.0d); }

        // This has to be gt epsilon, not gt zero to avoid glitches.
        double csq = lab.a * lab.a + lab.b * lab.b;
        if (csq > float.Epsilon)
        {
            double C = Math.Sqrt(csq);
            double a_ = lab.a / C;
            double b_ = lab.b / C;

            // 1.0 / math.pi = 0.3183098861837907
            double h = 0.5d + 0.5d * (Math.Atan2(-lab.b, -lab.a) * 0.3183098861837907d);

            var cusp = FindCusp(a_, b_);
            var stMax = ToSt(cusp);
            double sMax = stMax.S;
            double tMax = stMax.T;
            double s0 = 0.5d;
            double k = 1.0d;
            if (sMax != 0.0d) { k = 1.0d - s0 / sMax; }

            // first we find L_v, C_v, L_vt and C_vt
            double tDenom = C + L * tMax;
            double t = 0.0d;
            if (tDenom != 0.0d) { t = tMax / tDenom; }
            double lv = t * L;
            double cv = t * C;

            double lvt = ToeInv(lv);
            double cvt = 0.0d;
            if (lv != 0.0d) { cvt = cv * lvt / lv; }

            // we can then use these to invert the step that compensates for the toe and the curved top part of the triangle:
            var rgbScale = OkLabToLinearSrgb(
                (L: lvt,
                    a: a_ * cvt,
                    b: b_ * cvt));
            double scaleDenom = Math.Max(rgbScale.r, Math.Max(rgbScale.g, Math.Max(rgbScale.b, 0.0d)));
            double lScale = 0.0d;
            if (scaleDenom != 0.0d)
            {
                lScale = Math.Pow(1.0d / scaleDenom, 1.0d / 3.0d);
                L = L / lScale;
                C = C / lScale;
            }

            double toel = Toe(L);
            C = C * toel / L;
            L = toel;

            // we can now compute v and s:
            double v = 0.0d;
            if (lv != 0.0d) { v = L / lv; }

            double s = 0.0d;
            double sDenom = tMax * s0 + tMax * k * cv;
            if (sDenom != 0.0d) { s = (s0 + tMax) * cv / sDenom; }

            return (h, s, v);
        }
        else
        {
            return (h: 0.0d, s: 0.0d, v: L);
        }
    }

    public static (double r, double g, double b) OkLabToSrgb(in (double L, double a, double b) lab)
    {
        var lrgb = OkLabToLinearSrgb(lab);
        return (
            r: SrgbTransferFunction(lrgb.r),
            g: SrgbTransferFunction(lrgb.g),
            b: SrgbTransferFunction(lrgb.b));
    }

    public static (double h, double s, double l) SrgbToOkHsl(in (double r, double g, double b) srgb)
    {
        return OkLabToOkHsl(SrgbToOkLab(srgb));
    }

    public static (double h, double s, double v) SrgbToOkHsv(in (double r, double g, double b) srgb)
    {
        return OkLabToOkHsv(SrgbToOkLab(srgb));
    }

    public static (double L, double a, double b) SrgbToOkLab(in (double r, double g, double b) srgb)
    {
        return LinearSrgbToOkLab((
            r: SrgbTransferFunctionInv(srgb.r),
            g: SrgbTransferFunctionInv(srgb.g),
            b: SrgbTransferFunctionInv(srgb.b)));
    }

    static double SrgbTransferFunction(in double a)
    {
        return 0.0031308d >= a ? 12.92d * a : 1.055d * Math.Pow(a, 1.0d / 2.4d) - 0.055d;
    }

    static double SrgbTransferFunctionInv(in double a)
    {
        return 0.04045d < a ? Math.Pow((a + 0.055d) / 1.055d, 2.4d) : a / 12.92d;
    }

    static (double S, double T) ToSt(in (double L, double C) cusp)
    {
        double L = cusp.L;
        double C = cusp.C;
        if (L != 0.0d && L != 1.0d)
        {
            return (S: C / L, T: C / (1.0d - L));
        }
        else if (L != 0.0d)
        {
            return (S: C / L, T: 0.0d);
        }
        else if (L != 1.0d)
        {
            return (S: 0.0d, T: C / (1.0d - L));
        }
        else
        {
            return (S: 0.0d, T: 0.0d);
        }
    }

    static double Toe(in double x)
    {
        double y = 1.170873786407767d * x - 0.206d;
        return 0.5d * (y + Math.Sqrt(y * y + 0.14050485436893204d * x));
    }

    static double ToeInv(in double x)
    {
        double denom = 1.170873786407767d * (x + 0.03d);
        return denom != 0.0 ? (x * x + 0.206d * x) / denom : 0.0d;
    }
}