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

namespace MaterialColorUtilities.Schemes;

/// <summary>
/// Represents a Material color scheme, a mapping of color roles to colors.
/// </summary>
/// <typeparam name="TColor">The type of the named colors.</typeparam>
public class Scheme<TColor>
{
    public TColor Primary { get; set; }
    public TColor OnPrimary { get; set; }
    public TColor PrimaryContainer { get; set; }
    public TColor OnPrimaryContainer { get; set; }
    public TColor Secondary { get; set; }
    public TColor OnSecondary { get; set; }
    public TColor SecondaryContainer { get; set; }
    public TColor OnSecondaryContainer { get; set; }
    public TColor Tertiary { get; set; }
    public TColor OnTertiary { get; set; }
    public TColor TertiaryContainer { get; set; }
    public TColor OnTertiaryContainer { get; set; }
    public TColor Error { get; set; }
    public TColor OnError { get; set; }
    public TColor ErrorContainer { get; set; }
    public TColor OnErrorContainer { get; set; }
    public TColor Background { get; set; }
    public TColor OnBackground { get; set; }
    public TColor Surface { get; set; }
    public TColor OnSurface { get; set; }
    public TColor SurfaceVariant { get; set; }
    public TColor OnSurfaceVariant { get; set; }
    public TColor Outline { get; set; }
    public TColor Shadow { get; set; }
    public TColor InverseSurface { get; set; }
    public TColor InverseOnSurface { get; set; }
    public TColor InversePrimary { get; set; }

    /// <summary>
    /// Converts the Scheme into a new one with a different color type.
    /// </summary>
    /// <typeparam name="TResult">The color type of the result Scheme</typeparam>
    /// <param name="convert">The function used to convert <typeparamref name="TColor"/> to <typeparamref name="TResult"/></param>
    /// <remarks>
    /// In derived classes this will be replaced by a source generated method with an appropriate return type
    /// and including mapping for new colors.
    /// Source generation will only work if the class is marked partial,
    /// isn't nested inside another class
    /// and has a generic type parameter for at least the color type.
    /// </remarks>
    public Scheme<TResult> ConvertTo<TResult>(Func<TColor, TResult> convert)
    {
        return ConvertTo(convert, new());
    }

    /// <summary>
    /// Maps the Scheme's colors onto an existing Scheme object with a different color type.
    /// </summary>
    /// <typeparam name="TResult">The color type of the result Scheme</typeparam>
    /// <param name="convert">The function used to convert <typeparamref name="TColor"/> to <typeparamref name="TResult"/></param>
    /// <param name="result">The Scheme that will contain the converted colors</param>
    /// <returns><paramref name="result"/></returns>
    /// <remarks>
    /// Derived classes will have this source generated with an appropriate return type
    /// and including mapping for new colors.
    /// Source generation will only work if the class is marked partial,
    /// isn't nested inside another class
    /// and has a generic type parameter for at least the color type.
    /// </remarks>
    public Scheme<TResult> ConvertTo<TResult>(Func<TColor, TResult> convert, Scheme<TResult> result)
    {
        result.Primary = convert(Primary);
        result.OnPrimary = convert(OnPrimary);
        result.PrimaryContainer = convert(PrimaryContainer);
        result.OnPrimaryContainer = convert(OnPrimaryContainer);
        result.Secondary = convert(Secondary);
        result.OnSecondary = convert(OnSecondary);
        result.SecondaryContainer = convert(SecondaryContainer);
        result.OnSecondaryContainer = convert(OnSecondaryContainer);
        result.Tertiary = convert(Tertiary);
        result.OnTertiary = convert(OnTertiary);
        result.TertiaryContainer = convert(TertiaryContainer);
        result.OnTertiaryContainer = convert(OnTertiaryContainer);
        result.Error = convert(Error);
        result.OnError = convert(OnError);
        result.ErrorContainer = convert(ErrorContainer);
        result.OnErrorContainer = convert(OnErrorContainer);
        result.Background = convert(Background);
        result.OnBackground = convert(OnBackground);
        result.Surface = convert(Surface);
        result.OnSurface = convert(OnSurface);
        result.SurfaceVariant = convert(SurfaceVariant);
        result.OnSurfaceVariant = convert(OnSurfaceVariant);
        result.Outline = convert(Outline);
        result.Shadow = convert(Shadow);
        result.InverseSurface = convert(InverseSurface);
        result.InverseOnSurface = convert(InverseOnSurface);
        result.InversePrimary = convert(InversePrimary);
        return result;
    }
}
