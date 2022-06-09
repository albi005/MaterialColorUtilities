using Playground.Maui.Services;

namespace Playground.Maui;

public partial class App : Application
{
    public App(ThemeService themeService)
    {
        InitializeComponent();

        themeService.Apply();
        RequestedThemeChanged += (sender, args) => themeService.Apply();

        MainPage = new AppShell();
    }
}