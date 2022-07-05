namespace MaterialColorUtilities.Maui;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseMaterialDynamicColors(this MauiAppBuilder builder, Action<DynamicColorOptions> configureOptions = null)
    {
        builder.Services.Configure(configureOptions ?? (_ => { }));
        builder.Services.AddSingleton<DynamicColorService>();
        builder.Services.AddSingleton<IMauiInitializeService, InitializeService>();
        return builder;
    }
}
