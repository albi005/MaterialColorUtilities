using MaterialColorUtilities.Maui;
using Playground.Maui.ViewModels;

namespace Playground.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp
            .CreateBuilder()
            .UseMaterialDynamicColors()
            .UseMauiApp<App>();

        builder.Services.AddTransient<ThemeViewModel>();
        builder.Services.AddTransient<ThemePage>();

        return builder.Build();
    }
}