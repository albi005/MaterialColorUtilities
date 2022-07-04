using Microsoft.Maui.LifecycleEvents;

namespace MaterialColorUtilities.Maui;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseMaterialDynamicColors(this MauiAppBuilder builder, Action<DynamicColorOptions> configureOptions = null)
    {
        builder.Services.Configure(configureOptions ?? (_ => { }));

        DynamicColorService service = new();
        builder.Services.AddSingleton<IMauiInitializeService, DynamicColorService>((_) => service);
        builder.Services.AddSingleton(service);

        builder.ConfigureLifecycleEvents(builder =>
        {
#if ANDROID
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                builder.AddAndroid(builder =>
                {
#pragma warning disable CA1416
                    builder.OnResume(_ => service.SetFromAndroid12AccentColors());
#pragma warning restore CA1416
                });
            }
#endif
        });

        return builder;
    }
}
