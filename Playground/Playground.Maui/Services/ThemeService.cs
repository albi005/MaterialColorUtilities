using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Samples.Shared;
using MaterialColorUtilities.Schemes;

namespace MaterialColorUtilities.Samples.Maui.Services;

public class ThemeService
{
    private int seed = Score.Scorer.Default;

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
}
