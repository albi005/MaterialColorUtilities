using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MaterialColorUtilities.Maui;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseMaterialColors(this MauiAppBuilder builder)
        => builder.UseMaterialColors(_ => { });

    public static MauiAppBuilder UseMaterialColors(this MauiAppBuilder builder, uint fallbackSeed)
        => builder.UseMaterialColors(opt => opt.FallbackSeed = fallbackSeed);

    public static MauiAppBuilder UseMaterialColors(
        this MauiAppBuilder builder,
        Action<MaterialColorOptions> configureOptions)
        => builder.UseMaterialColors<MaterialColorService>(configureOptions);

    public static MauiAppBuilder UseMaterialColors<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
            TMaterialColorService>
        (this MauiAppBuilder builder)
        where TMaterialColorService : class, IMaterialColorService
        => builder.UseMaterialColors<TMaterialColorService>(_ => { });

    public static MauiAppBuilder UseMaterialColors<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TMaterialColorService>
    (
        this MauiAppBuilder builder, uint fallbackSeed
    )
        where TMaterialColorService : class, IMaterialColorService
        => builder.UseMaterialColors<TMaterialColorService>(opt => opt.FallbackSeed = fallbackSeed);

    public static MauiAppBuilder UseMaterialColors<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TMaterialColorService>
    (
        this MauiAppBuilder builder,
        Action<MaterialColorOptions> configureOptions
    )
        where TMaterialColorService : class, IMaterialColorService
    {
        builder.Services.Configure(configureOptions);
        builder.Services.TryAddSingleton(_ => Preferences.Default);
        builder.Services.AddSingleton<IDynamicColorService, DynamicColorService>();
        builder.Services.AddSingleton<TMaterialColorService>();
        builder.Services.AddSingleton<IMauiInitializeService, TMaterialColorService>(s => s.GetRequiredService<TMaterialColorService>());
        return builder;
    }
}