using CommunityToolkit.Mvvm.ComponentModel;
using MaterialColorUtilities.ColorAppearance;
using Playground.Maui.Services;

namespace Playground.Maui.ViewModels;

public partial class ThemeViewModel : ObservableObject
{
    private readonly ThemeService _themeService;
    [ObservableProperty] private double _h;
    [ObservableProperty] private double _c = 100;
    [ObservableProperty] private double _t = 50;
    [ObservableProperty] private Color _seed;

    public ThemeViewModel(ThemeService themeService)
    {
        _themeService = themeService;
        Hct hct = Hct.FromInt(_themeService.Seed);
        _h = hct.Hue;
        _c = hct.Chroma;
        _t = hct.Tone;
        OnHctChanged();
    }

    partial void OnHChanged(double value) => OnHctChanged();
    partial void OnCChanged(double value) => OnHctChanged();
    partial void OnTChanged(double value) => OnHctChanged();

    void OnHctChanged()
    {
        Hct hct = Hct.From(H, C, T);
        Seed = Color.FromInt(hct.ToInt());
        _themeService.Seed = hct.ToInt();
    }
}
