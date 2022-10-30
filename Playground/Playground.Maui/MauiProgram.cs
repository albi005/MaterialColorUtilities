using MaterialColorUtilities.Maui;
using Playground.Maui.ViewModels;
using Style = MaterialColorUtilities.Palettes.Style;

namespace Playground.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp
            .CreateBuilder()
            .UseMaterialColors<CustomMaterialColorService>(opt =>
            {
                opt.DefaultStyle = Style.Expressive;
            })
            .UseMauiApp<App>();

        builder.Services.AddTransient<ThemeViewModel>();
        builder.Services.AddTransient<ThemePage>();

        return builder.Build();
    }
}