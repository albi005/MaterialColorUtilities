using Windows.UI.ViewManagement;
using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Maui;

public class SeedColorService : ISeedColorService
{
    private readonly UISettings _uiSettings = new();

    public SeedColorService()
    {
        _uiSettings.ColorValuesChanged += (_, _) => OnSeedColorChanged?.Invoke();
    }
    
    public int? SeedColor
    {
        get
        {
            Windows.UI.Color color = _uiSettings.GetColorValue(UIColorType.Accent);
            return ColorUtils.ArgbFromRgb(color.R, color.G, color.B);
        }
    }

    public event Action OnSeedColorChanged;
}