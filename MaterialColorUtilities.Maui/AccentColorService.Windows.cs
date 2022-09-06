using Windows.UI.ViewManagement;
using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Maui;

public class AccentColorService : IAccentColorService
{
    private readonly UISettings _uiSettings = new();

    public AccentColorService()
    {
        _uiSettings.ColorValuesChanged += (_, _) => OnAccentColorChanged?.Invoke();
    }
    
    public int? AccentColor
    {
        get
        {
            Windows.UI.Color color = _uiSettings.GetColorValue(UIColorType.Accent);
            return ColorUtils.ArgbFromRgb(color.R, color.G, color.B);
        }
    }

    public event Action OnAccentColorChanged;
}