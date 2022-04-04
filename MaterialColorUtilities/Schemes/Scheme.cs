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

    public Scheme<TResult> ConvertTo<TResult>(Func<TColor, TResult> convert)
    {
        return ConvertTo(convert, new());
    }

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
