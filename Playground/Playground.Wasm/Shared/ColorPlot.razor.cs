using Playground.Wasm.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using Playground.Wasm.Services;

namespace Playground.Wasm.Shared;

public partial class ColorPlot : SeedColorSelector
{
    private SKBitmap bitmap;
    private SKCanvasView view;
    private double seedX = -10000000;
    private double seedY = -10000000;
    private readonly SKPaint seedPaint = new()
    {
        Color = SKColors.White,
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2,
        IsAntialias = true
    };

    /// <summary>
    /// The width of the bitmap that will be generated.
    /// </summary>
    [Parameter] public int Resolution { get; set; } = 400;

    [Parameter] public Func<double, double, int> Func { get; set; }
    [Parameter] public Func<int, (double, double)> FuncInverse { get; set; }
    [Parameter] public double MinX { get; set; } = 0;
    [Parameter] public double MinY { get; set; } = 0;
    [Parameter] public double MaxX { get; set; } = 100;
    [Parameter] public double MaxY { get; set; } = 100;
    [Parameter] public string LabelX { get; set; } = "X";
    [Parameter] public string LabelY { get; set; } = "Y";

    protected override void OnInitialized()
    {
        bitmap = new(Resolution, Resolution / 2);
        for (int x = 0; x < bitmap.Width; x++)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                int color = GetColor(x, y);
                bitmap.SetPixel(x, bitmap.Height - 1 - y, (uint)color);
            }
        }
        bitmap = bitmap.Resize(new SKImageInfo(400, 200), SKFilterQuality.None);
        base.OnInitialized();
    }

    public void OnPaintSurface(SKPaintSurfaceEventArgs args)
    {
        var canvas = args.Surface.Canvas;
        canvas.Clear();
        args.DrawLabels(MinX, MinY, MaxX, MaxY, LabelX, LabelY, ThemeService.Scheme);
        canvas.DrawBitmap(bitmap, 20, 20);
        canvas.DrawCircle((float)(seedX + 20), (float)(bitmap.Height - seedY + 20), 6, seedPaint);
    }

    public void OnClick(MouseEventArgs args)
    {
        if (!(20 < args.OffsetX
            && args.OffsetX < bitmap.Width + 20
            && 20 < args.OffsetY
            && args.OffsetY < bitmap.Height + 20))
            return;
        seedX = args.OffsetX - 20;
        seedY = bitmap.Height - (args.OffsetY - 20);
        int color = GetColor(seedX, seedY);
        themeService.SetSeed(color, this);
        view?.Invalidate();
    }

    protected override void SetFromSeed(int seed)
    {
        if (FuncInverse == null) return;
        var xy = FuncInverse(seed);
        double percentX = (xy.Item1 - MinX) / (MaxX - MinX);
        double percentY = (xy.Item2 - MinY) / (MaxY - MinY);
        seedX = percentX * bitmap.Width;
        seedY = percentY * bitmap.Height;
        view?.Invalidate();
    }

    private int GetColor(double x, double y)
        => Func(
            x / bitmap.Width * (MaxX - MinX) + MinX,
            y / bitmap.Height * (MaxY - MinY) + MinY
        );
}