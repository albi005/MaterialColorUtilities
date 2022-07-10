using Foundation;
using MaterialColorUtilities.Utils;
using System.Runtime.InteropServices;
using UIKit;

namespace MaterialColorUtilities.Maui;

// NSColor.controlAccentColor is not available because .NET MAUI uses Mac Catalyst,
// so we have to rely on a workaround to get the accent color and subscribe to its changes.
partial class DynamicColorService<TCorePalette, TSchemeInt, TSchemeMaui, TLightSchemeMapper, TDarkSchemeMapper>
{
    private readonly UIButton _dummy = new();

    partial void PlatformInitialize()
    {
        SetFromAccentColor();

        try
        {
            // based on https://gist.github.com/JunyuKuang/3ecc7c9374c0ba67438c9a6d06612e36
            NSNotificationCenter.DefaultCenter.AddObserver(
                (NSString)"NSSystemColorsDidChangeNotification",
                _ => MainThread.BeginInvokeOnMainThread(SetFromAccentColor),
                null);
        }
        catch { }
    }

    // https://twitter.com/steipete/status/1186262035543273472
    private void SetFromAccentColor()
    {
        try
        {
            UIColor accentColor = _dummy.TintColor;
            accentColor.GetRGBA(
                out NFloat r,
                out NFloat g,
                out NFloat b,
                out NFloat _);
            int argb = ColorUtils.ArgbFromRgb(
                (int)(r * 255),
                (int)(g * 255),
                (int)(b * 255));
            SetSeed(argb);
        }
        catch { }
    }
}
