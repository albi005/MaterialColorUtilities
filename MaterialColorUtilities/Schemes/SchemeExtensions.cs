using System;

namespace MaterialColorUtilities.Schemes;

public static class SchemeExtensions
{
    public static Scheme<T> Convert<R, T>(this Scheme<R> r, Func<R, T> f)
    {
        var t = new Scheme<T>
        {
            Primary = f(r.Primary),
            OnPrimary = f(r.OnPrimary),
            PrimaryContainer = f(r.PrimaryContainer),
            OnPrimaryContainer = f(r.OnPrimaryContainer),
            Secondary = f(r.Secondary),
            OnSecondary = f(r.OnSecondary),
            SecondaryContainer = f(r.SecondaryContainer),
            OnSecondaryContainer = f(r.OnSecondaryContainer),
            Tertiary = f(r.Tertiary),
            OnTertiary = f(r.OnTertiary),
            TertiaryContainer = f(r.TertiaryContainer),
            OnTertiaryContainer = f(r.OnTertiaryContainer),
            Error = f(r.Error),
            OnError = f(r.OnError),
            ErrorContainer = f(r.ErrorContainer),
            OnErrorContainer = f(r.OnErrorContainer),
            Background = f(r.Background),
            OnBackground = f(r.OnBackground),
            Surface = f(r.Surface),
            OnSurface = f(r.OnSurface),
            SurfaceVariant = f(r.SurfaceVariant),
            OnSurfaceVariant = f(r.OnSurfaceVariant),
            Outline = f(r.Outline),
            Shadow = f(r.Shadow),
            InverseSurface = f(r.InverseSurface),
            InverseOnSurface = f(r.InverseOnSurface),
            InversePrimary = f(r.InversePrimary),
            Surface1 = f(r.Surface1),
            Surface2 = f(r.Surface2),
            Surface3 = f(r.Surface3),
            Surface4 = f(r.Surface4),
            Surface5 = f(r.Surface5),
            SurfaceDim = f(r.SurfaceDim),
            SurfaceBright = f(r.SurfaceBright),
            SurfaceContainerLowest = f(r.SurfaceContainerLowest),
            SurfaceContainerLow = f(r.SurfaceContainerLow),
            SurfaceContainer = f(r.SurfaceContainer),
            SurfaceContainerHigh = f(r.SurfaceContainerHigh),
            SurfaceContainerHighest = f(r.SurfaceContainerHighest),
            OutlineVariant = f(r.OutlineVariant)
        };
        return t;
    }
}
