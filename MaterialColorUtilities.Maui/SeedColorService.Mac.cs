using System.Runtime.InteropServices;
using Foundation;
using MaterialColorUtilities.Utils;
using UIKit;

namespace MaterialColorUtilities.Maui;

public class SeedColorService : ISeedColorService
{
    private readonly UIButton _dummyButton = new();

    public SeedColorService()
    {
        // based on https://gist.github.com/JunyuKuang/3ecc7c9374c0ba67438c9a6d06612e36
        NSNotificationCenter.DefaultCenter.AddObserver(
            (NSString)"NSSystemColorsDidChangeNotification",
            _ => OnSeedColorChanged?.Invoke(),
            null);
    }
    
    public uint? SeedColor
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
                (uint)(r * 255),
                (uint)(g * 255),
                (uint)(b * 255));
        }
    }

    public event Action OnSeedColorChanged;
}