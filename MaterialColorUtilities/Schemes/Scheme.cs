namespace MaterialColorUtilities.Schemes;

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
}
