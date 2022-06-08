using MaterialColorUtilities.ColorAppearance;
using Playground.Maui.Components;

namespace Playground.Maui;

public partial class GraphsPage : ContentPage
{
    public GraphsPage()
    {
        InitializeComponent();

        Plot((x, y) => Hct.From(x, 100, y).ToInt(),
            "HCT, chroma = 100",
            "Hue", 360,
            "Tone", 100);
        Plot((x, y) => Cam16.FromJch(y, 100, x).ToInt(),
            "CAM16 from JCH, chroma = 100",
            "Hue", 360,
            "Lightness", 100);
        Plot((x, y) => Color.FromHsla(x, 1, y).ToInt(),
            "HSL, saturation = 1",
            "Hue", 1,
            "Lightness", 1);
    }

    private void Plot(Func<double, double, int> func, string title, string xLabel, double xMax, string yLabel, double yMax)
    {
        stack.Add(new Label { Text = title });
        stack.Add(new ColorPlot(func, xLabel, xMax, yLabel, yMax) { Margin = new Thickness(0, 0, 0, 32) });
    }
}