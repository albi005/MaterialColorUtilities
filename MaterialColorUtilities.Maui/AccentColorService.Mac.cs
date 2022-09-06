using System.Runtime.InteropServices;
using Foundation;
using MaterialColorUtilities.Utils;
using UIKit;

namespace MaterialColorUtilities.Maui;

public class AccentColorService : IAccentColorService
{
    private readonly UIButton _dummyButton = new();

    public AccentColorService()
    {
        // based on https://gist.github.com/JunyuKuang/3ecc7c9374c0ba67438c9a6d06612e36
        NSNotificationCenter.DefaultCenter.AddObserver(
            (NSString)"NSSystemColorsDidChangeNotification",
            _ => OnAccentColorChanged?.Invoke(),
            null);
    }
    
    public int? AccentColor
    {
        get
        {
            UIColor accentColor = _dummyButton.TintColor;
            if (accentColor == null) return null;
            accentColor.GetRGBA(
                out NFloat r,
                out NFloat g,
                out NFloat b,
                out NFloat _);
            return ColorUtils.ArgbFromRgb(
                (int)(r * 255),
                (int)(g * 255),
                (int)(b * 255));
        }
    }

    public event Action OnAccentColorChanged;
}