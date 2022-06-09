using Playground.Maui.Services;
using Playground.Maui.ViewModels;

namespace Playground.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp
            .CreateBuilder()
            .UseMauiApp<App>();

        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddTransient<ThemeViewModel>();
        builder.Services.AddTransient<ThemePage>();

        return builder.Build();
    }
}