using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Maui;
using Microsoft.Extensions.Options;

namespace Playground.Maui.ViewModels;

public partial class ThemeViewModel : ObservableObject
{
    private readonly CustomDynamicColorService _colorService;
    
    [ObservableProperty] private double _h;
    [ObservableProperty] private double _c;
    [ObservableProperty] private double _t;
    [ObservableProperty] private Color _seed;
    
    [ObservableProperty] private bool _enableTheming;
    [ObservableProperty] private bool _enableDynamicColor;

    public AppTheme UserAppTheme
    {
        get => Application.Current!.UserAppTheme;
        set => Application.Current!.UserAppTheme = value;
    }

    public List<AppTheme> ThemeOptions { get; } = new()
    {
        AppTheme.Unspecified,
        AppTheme.Light,
        AppTheme.Dark
    };

    public Color OnSeed => _t < 49.6 ? Colors.White : Colors.Black;

    public ThemeViewModel(CustomDynamicColorService colorService, IOptions<DynamicColorOptions> options)
    {
        _colorService = colorService;

        _enableTheming = options.Value.EnableTheming;
        _enableDynamicColor = options.Value.EnableDynamicColor;
        
        _colorService.SeedChanged += (_, _) =>
        {
            if (_colorService.Seed == _seed?.ToInt()) return;
            MainThread.BeginInvokeOnMainThread(SetFromSeed);
        };
        SetFromSeed();
    }

    partial void OnHChanged(double value) => SetSeed();
    partial void OnCChanged(double value) => SetSeed();
    partial void OnTChanged(double value) => SetSeed();
    partial void OnSeedChanged(Color value) => OnPropertyChanged(nameof(OnSeed));
    
    partial void OnEnableThemingChanged(bool value) => _colorService.IsEnabled = value;
    partial void OnEnableDynamicColorChanged(bool value) => _colorService.EnableDynamicColor = value;

    [ICommand]
    void ForgetSeed() => _colorService.ForgetSeed();
    
    void SetSeed()
    {
        Hct hct = Hct.From(H, C, T);
        Seed = Color.FromInt(hct.ToInt());
        _colorService.Seed = hct.ToInt();
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
