using MaterialColorUtilities.Utils;
using Windows.UI.ViewManagement;

namespace MaterialColorUtilities.Maui;

public partial class DynamicColorService
{
    private readonly UISettings uiSettings = new();

    partial void PlatformInitialize()
    {
        SetSeed(GetAccentColor());
        uiSettings.ColorValuesChanged += (_, _)
            => MainThread.BeginInvokeOnMainThread(()
            => SetSeed(GetAccentColor()));
    }

    private int GetAccentColor()
    {
        Windows.UI.Color color = uiSettings.GetColorValue(UIColorType.Accent);
        return ColorUtils.ArgbFromRgb(color.R, color.G, color.B);
    }
}
