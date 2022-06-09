using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Schemes;
using MaterialColorUtilities.Score;
using MaterialColorUtilities.Utils;
using Playground.Shared;

namespace Playground.Maui.Services;

public partial class ThemeService
{
    private int seed = Scorer.Default;

    public int Seed
    {
        get => seed;
        set { seed = value; Apply(); }
    }
    public AppScheme<Color> Scheme { get; private set; }

    public void Apply()
    {
        bool isDark = Application.Current.RequestedTheme == AppTheme.Dark;

        CorePalette corePalette = new(Seed);

        ISchemeMapper<CorePalette, AppScheme<int>> mapper = isDark
            ? new DarkAppSchemeMapper()
            : new LightAppSchemeMapper();
        AppScheme<int> scheme = mapper.Map(corePalette);
        Scheme = scheme.ConvertTo(Color.FromInt);

        foreach (var property in Scheme.GetType().GetProperties())
        {
            string key = property.Name;
            Color value = (Color)property.GetValue(Scheme);
            Application.Current.Resources[key] = value;
            Application.Current.Resources[key + "Brush"] = new SolidColorBrush(value);
        }
    }

#if ANDROID || WINDOWS
    public async void TrySetFromWallpaper()
    {
        int[] pixels = await GetWallpaperPixels();
        if (pixels == null) return;
        int color = ImageUtils.ColorsFromImage(pixels).First();
        Seed = color;
    }
#endif

    public void Initialize(App app)
    {
        app.RequestedThemeChanged += (sender, args) => Apply();

        Apply();
    }
}
