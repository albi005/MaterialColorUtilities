using Windows.UI.ViewManagement;
using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Maui;

public class DynamicColorService : IDynamicColorService
{
    private readonly UISettings _uiSettings = new();

    public DynamicColorService()
    {
        _uiSettings.ColorValuesChanged += (_, _) => Changed?.Invoke();
    }
    
    public uint? SeedColor
    {
        get
        {
            Windows.UI.Color color = _uiSettings.GetColorValue(UIColorType.Accent);
            return ColorUtils.ArgbFromRgb(color.R, color.G, color.B);
        }
    }

    public event Action? Changed;
}