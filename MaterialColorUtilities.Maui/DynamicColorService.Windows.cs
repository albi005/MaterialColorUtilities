using MaterialColorUtilities.Utils;
using Windows.UI.ViewManagement;

namespace MaterialColorUtilities.Maui;

public partial class DynamicColorService
{
    private readonly UISettings _uiSettings = new();

    partial void PlatformInitialize()
    {
        if (!_options.UseDynamicColor) return;

        SetSeed(GetAccentColor());
        _uiSettings.ColorValuesChanged += (_, _)
            => MainThread.BeginInvokeOnMainThread(()
            => SetSeed(GetAccentColor()));
    }

    private int GetAccentColor()
    {
        Windows.UI.Color color = _uiSettings.GetColorValue(UIColorType.Accent);
        return ColorUtils.ArgbFromRgb(color.R, color.G, color.B);
    }
}
