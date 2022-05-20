using MaterialColorUtilities.Samples.Maui.Services;

namespace MaterialColorUtilities.Samples.Maui
{
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
}