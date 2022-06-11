using CommunityToolkit.Mvvm.ComponentModel;
using MaterialColorUtilities.ColorAppearance;
using Playground.Maui.Services;

namespace Playground.Maui.ViewModels;

public partial class ThemeViewModel : ObservableObject
{
    private readonly ThemeService _themeService;
    [ObservableProperty] private double _h;
    [ObservableProperty] private double _c;
    [ObservableProperty] private double _t;
    [ObservableProperty] private Color _seed;

    public ThemeViewModel(ThemeService themeService)
    {
        _themeService = themeService;
        _themeService.SeedChanged += (sender, _) =>
        {
            if (sender == this) return;
            MainThread.BeginInvokeOnMainThread(SetFromSeed);
        };
        SetFromSeed();
    }

    partial void OnHChanged(double value) => SetSeed();
    partial void OnCChanged(double value) => SetSeed();
    partial void OnTChanged(double value) => SetSeed();

    void SetSeed()
    {
        Hct hct = Hct.From(H, C, T);
        Seed = Color.FromInt(hct.ToInt());
        _themeService.SetSeed(hct.ToInt(), this);
    }

    void SetFromSeed()
    {
        Seed = Color.FromInt(_themeService.Seed);
        Hct hct = Hct.FromInt(_themeService.Seed);
        _h = hct.Hue;
        _c = hct.Chroma;
        _t = hct.Tone;
        OnPropertyChanged("");
    }
}
