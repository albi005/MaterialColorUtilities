using MaterialColorUtilities.Samples.Maui.Services;
using MaterialColorUtilities.Samples.Maui.ViewModels;

namespace MaterialColorUtilities.Samples.Maui
{
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
}