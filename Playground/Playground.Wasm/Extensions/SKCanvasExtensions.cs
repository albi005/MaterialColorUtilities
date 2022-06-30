using MaterialColorUtilities.Schemes;
using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace Playground.Wasm.Extensions;

public static class SKPaintSurfaceEventArgsExtensions
{
    private static readonly SKPaint stroke = new()
    {
        Color = new(0xFF787777),
        IsStroke = true,
    };
    private static readonly SKPaint left = new()
    {
        Color = new(0xFF787777),
        TextSize = 16,
        IsAntialias = true,
    };
    private static readonly SKPaint center = new()
    {
        Color = new(0xFF787777),
        TextAlign = SKTextAlign.Center,
        TextSize = 16,
        IsAntialias = true,
    };
    private static readonly SKPaint right = new()
    {
        Color = new(0xFF787777),
        TextAlign = SKTextAlign.Right,
        TextSize = 16,
        IsAntialias = true,
    };

    public static void DrawLabels(
        this SKPaintSurfaceEventArgs args,
        double minX,
        double minY,
        double maxX,
        double maxY,
        string labelX,
        string labelY,
        Scheme<int> scheme)
    {
        stroke.Color = left.Color = center.Color = right.Color = new((uint)scheme.Outline);

        var canvas = args.Surface.Canvas;
        float width = args.Info.Width;
        float height = args.Info.Height;
        canvas.DrawRect(19, 19, width - 39, height - 39, stroke);
        
        canvas.DrawText(minX.ToString(), 20, height - 4, left);
        canvas.DrawText(labelX, width / 2, height - 4, center);
        canvas.DrawText(maxX.ToString(), width - 20, height - 4, right);
        
        canvas.RotateDegrees(-90);
        canvas.DrawText(minY.ToString(), -(height - 20), 16, left);
        canvas.DrawText(labelY, -(height / 2), 16, center);
        canvas.DrawText(maxY.ToString(), -20, 16, right);
        canvas.RotateDegrees(90);
    }
}
