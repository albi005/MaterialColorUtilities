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
public partial class Scheme<TColor>
{
    public TColor Primary { get; set; } = default!;
    public TColor OnPrimary { get; set; } = default!;
    public TColor PrimaryContainer { get; set; } = default!;
    public TColor OnPrimaryContainer { get; set; } = default!;
    public TColor Secondary { get; set; } = default!;
    public TColor OnSecondary { get; set; } = default!;
    public TColor SecondaryContainer { get; set; } = default!;
    public TColor OnSecondaryContainer { get; set; } = default!;
    public TColor Tertiary { get; set; } = default!;
    public TColor OnTertiary { get; set; } = default!;
    public TColor TertiaryContainer { get; set; } = default!;
    public TColor OnTertiaryContainer { get; set; } = default!;
    public TColor Error { get; set; } = default!;
    public TColor OnError { get; set; } = default!;
    public TColor ErrorContainer { get; set; } = default!;
    public TColor OnErrorContainer { get; set; } = default!;
    public TColor Background { get; set; } = default!;
    public TColor OnBackground { get; set; } = default!;
    public TColor Surface { get; set; } = default!;
    public TColor OnSurface { get; set; } = default!;
    public TColor SurfaceVariant { get; set; } = default!;
    public TColor OnSurfaceVariant { get; set; } = default!;
    public TColor Outline { get; set; } = default!;
    public TColor Shadow { get; set; } = default!;
    public TColor InverseSurface { get; set; } = default!;
    public TColor InverseOnSurface { get; set; } = default!;
    public TColor InversePrimary { get; set; } = default!;
    public TColor Surface1 { get; set; } = default!;
    public TColor Surface2 { get; set; } = default!;
    public TColor Surface3 { get; set; } = default!;
    public TColor Surface4 { get; set; } = default!;
    public TColor Surface5 { get; set; } = default!;
}
