using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MaterialColorUtilities.Maui;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseMaterialDynamicColors(this MauiAppBuilder builder)
        => builder.UseMaterialDynamicColors(_ => { });

    public static MauiAppBuilder UseMaterialDynamicColors(this MauiAppBuilder builder, uint fallbackSeed)
        => builder.UseMaterialDynamicColors(opt => opt.FallbackSeed = fallbackSeed);

    public static MauiAppBuilder UseMaterialDynamicColors(
        this MauiAppBuilder builder,
        Action<DynamicColorOptions> configureOptions)
        => builder.UseMaterialDynamicColors<DynamicColorService>(configureOptions);

    public static MauiAppBuilder UseMaterialDynamicColors<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
            TDynamicColorService>
        (this MauiAppBuilder builder)
        where TDynamicColorService : class, IMauiInitializeService
        => builder.UseMaterialDynamicColors<TDynamicColorService>(_ => { });

    public static MauiAppBuilder UseMaterialDynamicColors<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TDynamicColorService>
    (
        this MauiAppBuilder builder, uint fallbackSeed
    )
        where TDynamicColorService : class, IMauiInitializeService
        => builder.UseMaterialDynamicColors<TDynamicColorService>(opt => opt.FallbackSeed = fallbackSeed);

    public static MauiAppBuilder UseMaterialDynamicColors<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TDynamicColorService>
    (
        this MauiAppBuilder builder,
        Action<DynamicColorOptions> configureOptions
    )
        where TDynamicColorService : class, IMauiInitializeService
    {
        builder.Services.Configure(configureOptions);
        builder.Services.TryAddSingleton(_ => Preferences.Default);
        builder.Services.AddSingleton<ISeedColorService, SeedColorService>();
        builder.Services.AddSingleton<TDynamicColorService>();
        builder.Services.AddSingleton<IMauiInitializeService>(s => s.GetRequiredService<TDynamicColorService>());
        return builder;
    }
}