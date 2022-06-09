using Playground.Maui.Services;

namespace Playground.Maui;

public partial class App : Application
{
    public App(ThemeService themeService)
    {
        InitializeComponent();

        themeService.Initialize(this);

        MainPage = new AppShell(themeService);
    }
}