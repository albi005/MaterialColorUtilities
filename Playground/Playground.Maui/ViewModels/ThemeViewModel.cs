using CommunityToolkit.Mvvm.ComponentModel;
using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Maui;

namespace Playground.Maui.ViewModels;

public partial class ThemeViewModel : ObservableObject
{
    private readonly DynamicColorService _colorService;
    [ObservableProperty] private double _h;
    [ObservableProperty] private double _c;
    [ObservableProperty] private double _t;
    [ObservableProperty] private Color _seed;
    public Color OnSeed => _t < 49.6 ? Colors.White : Colors.Black;

    public ThemeViewModel(DynamicColorService themeService)
    {
        _colorService = themeService;
        _colorService.SeedChanged += (sender, _) =>
        {
            if (sender == this) return;
            MainThread.BeginInvokeOnMainThread(SetFromSeed);
        };
        SetFromSeed();
    }

    partial void OnHChanged(double value) => SetSeed();
    partial void OnCChanged(double value) => SetSeed();
    partial void OnTChanged(double value) => SetSeed();
    partial void OnSeedChanged(Color value) => OnPropertyChanged(nameof(OnSeed));

    void SetSeed()
    {
        Hct hct = Hct.From(H, C, T);
        Seed = Color.FromInt(hct.ToInt());
        _colorService.SetSeed(hct.ToInt(), this);
    }

    void SetFromSeed()
    {
        Seed = Color.FromInt(_colorService.Seed);
        Hct hct = Hct.FromInt(_colorService.Seed);
        _h = hct.Hue;
        _c = hct.Chroma;
        _t = hct.Tone;
        OnPropertyChanged("");
    }
}
