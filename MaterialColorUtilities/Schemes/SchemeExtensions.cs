namespace MaterialColorUtilities.Schemes;

public static class SchemeExtensions
{
    public static Scheme<TTo> Convert<TFrom, TTo>(this Scheme<TFrom> from, Func<TFrom, TTo> convert) => new()
    {
        Primary = convert(from.Primary),
        OnPrimary = convert(from.OnPrimary),
        PrimaryContainer = convert(from.PrimaryContainer),
        OnPrimaryContainer = convert(from.OnPrimaryContainer),
        Secondary = convert(from.Secondary),
        OnSecondary = convert(from.OnSecondary),
        SecondaryContainer = convert(from.SecondaryContainer),
        OnSecondaryContainer = convert(from.OnSecondaryContainer),
        Tertiary = convert(from.Tertiary),
        OnTertiary = convert(from.OnTertiary),
        TertiaryContainer = convert(from.TertiaryContainer),
        OnTertiaryContainer = convert(from.OnTertiaryContainer),
        Error = convert(from.Error),
        OnError = convert(from.OnError),
        ErrorContainer = convert(from.ErrorContainer),
        OnErrorContainer = convert(from.OnErrorContainer),
        Background = convert(from.Background),
        OnBackground = convert(from.OnBackground),
        Surface = convert(from.Surface),
        OnSurface = convert(from.OnSurface),
        SurfaceVariant = convert(from.SurfaceVariant),
        OnSurfaceVariant = convert(from.OnSurfaceVariant),
        Outline = convert(from.Outline),
        Shadow = convert(from.Shadow),
        InverseSurface = convert(from.InverseSurface),
        InverseOnSurface = convert(from.InverseOnSurface),
        InversePrimary = convert(from.InversePrimary),
    };
}