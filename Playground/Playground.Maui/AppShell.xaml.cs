using Playground.Maui.Services;

namespace Playground.Maui
{
    public partial class AppShell : Shell
    {
        private readonly ThemeService themeService;

        public AppShell(ThemeService themeService)
        {
            InitializeComponent();
            this.themeService = themeService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
#if ANDROID || WINDOWS
            themeService.TrySetFromWallpaper();
#endif        
        }
    }
}