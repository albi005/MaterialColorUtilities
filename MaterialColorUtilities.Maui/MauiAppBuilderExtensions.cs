using System.Diagnostics.CodeAnalysis;

namespace MaterialColorUtilities.Maui;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseMaterialDynamicColors(this MauiAppBuilder builder)
        => builder.UseMaterialDynamicColors(_ => { });

    public static MauiAppBuilder UseMaterialDynamicColors(this MauiAppBuilder builder, uint fallbackSeed)
        => builder.UseMaterialDynamicColors(opt => opt.FallbackSeed = (int)fallbackSeed);

    public static MauiAppBuilder UseMaterialDynamicColors(
        this MauiAppBuilder builder,
        Action<DynamicColorOptions> configureOptions)
        => builder.UseMaterialDynamicColors<DynamicColorService>(configureOptions);

    public static MauiAppBuilder UseMaterialDynamicColors<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
            TDynamicColorService>
        (this MauiAppBuilder builder)
        where TDynamicColorService : class, IDynamicColorService
        => builder.UseMaterialDynamicColors<TDynamicColorService>(_ => { });

    public static MauiAppBuilder UseMaterialDynamicColors<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TDynamicColorService>
    (
        this MauiAppBuilder builder, uint fallbackSeed
    )
        where TDynamicColorService : class, IDynamicColorService
        => builder.UseMaterialDynamicColors<TDynamicColorService>(opt => opt.FallbackSeed = (int)fallbackSeed);

    public static MauiAppBuilder UseMaterialDynamicColors<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TDynamicColorService>
    (
        this MauiAppBuilder builder,
        Action<DynamicColorOptions> configureOptions
    )
        where TDynamicColorService : class, IDynamicColorService
    {
        builder.Services.Configure(configureOptions);
        builder.Services.AddSingleton<TDynamicColorService>();
        builder.Services.AddSingleton<IMauiInitializeService, InitializeService<TDynamicColorService>>();
        return builder;
    }
}