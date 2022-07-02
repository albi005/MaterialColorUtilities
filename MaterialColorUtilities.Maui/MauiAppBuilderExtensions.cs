namespace MaterialColorUtilities.Maui;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseMaterialDynamicColors(this MauiAppBuilder builder, Action<DynamicColorOptions> configureOptions = null)
    {
        builder.Services.Configure(configureOptions ?? (_ => { }));

        builder.Services.AddSingleton<IMauiInitializeService, DynamicColorService>();
        builder.Services.AddSingleton<DynamicColorService>();

        return builder;
    }
}
