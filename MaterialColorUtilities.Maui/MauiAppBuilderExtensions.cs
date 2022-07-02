namespace MaterialColorUtilities.Maui;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseMaterialDynamicColors(this MauiAppBuilder builder, Action<DynamicColorOptions> configureOptions = null)
    {
        builder.Services.Configure(configureOptions ?? (_ => { }));

        DynamicColorService service = new();
        builder.Services.AddSingleton<IMauiInitializeService, DynamicColorService>((_) => service);
        builder.Services.AddSingleton(service);

        return builder;
    }
}
