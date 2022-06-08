using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace MaterialColorUtilities.Samples.Wasm.Extensions;

public static class SKPaintSurfaceEventArgsExtensions
{
    private static readonly SKPaint grayStroke = new()
    {
        Color = new(0xFF787777),
        IsStroke = true,
    };
    private static readonly SKPaint grayLeft = new()
    {
        Color = new(0xFF787777),
        TextSize = 16,
        IsAntialias = true,
    };
    private static readonly SKPaint grayCenter = new()
    {
        Color = new(0xFF787777),
        TextAlign = SKTextAlign.Center,
        TextSize = 16,
        IsAntialias = true,
    };
    private static readonly SKPaint grayRight = new()
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
        string labelY)
    {
        var canvas = args.Surface.Canvas;
        float width = args.Info.Width;
        float height = args.Info.Height;
        canvas.DrawRect(19, 19, width - 39, height - 39, grayStroke);
        
        canvas.DrawText(minX.ToString(), 20, height - 4, grayLeft);
        canvas.DrawText(labelX, width / 2, height - 4, grayCenter);
        canvas.DrawText(maxX.ToString(), width - 20, height - 4, grayRight);
        
        canvas.RotateDegrees(-90);
        canvas.DrawText(minY.ToString(), -(height - 20), 16, grayLeft);
        canvas.DrawText(labelY, -(height / 2), 16, grayCenter);
        canvas.DrawText(maxY.ToString(), -20, 16, grayRight);
        canvas.RotateDegrees(90);
    }
}
