using System.Collections.Concurrent;
using Playground.Wasm.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using Playground.Wasm.Services;

namespace Playground.Wasm.Shared;

public partial class ColorPlot : SeedColorSelector
{
    private SKBitmap _bitmap;
    private SKCanvasView _view;
    private double _seedX = -10000000;
    private double _seedY = -10000000;
    private readonly SKPaint _seedPaint = new()
    {
        Color = SKColors.White,
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 2,
        IsAntialias = true
    };

    private static readonly ConcurrentDictionary<string, SKBitmap> _bitmapCache = new();

    /// <summary>
    /// The width of the bitmap that will be generated.
    /// </summary>
    [Parameter] public int Resolution { get; set; } = 400;

    [Parameter] public Func<double, double, uint> Func { get; set; }
    [Parameter] public Func<uint, (double, double)> FuncInverse { get; set; }
    [Parameter] public double MinX { get; set; }
    [Parameter] public double MinY { get; set; }
    [Parameter] public double MaxX { get; set; } = 100;
    [Parameter] public double MaxY { get; set; } = 100;
    [Parameter] public string LabelX { get; set; } = "X";
    [Parameter] public string LabelY { get; set; } = "Y";
    [Parameter] public string CacheId { get; set; }

    protected override void OnInitialized()
    {
        if (CacheId == null) CreateBitmap();
        else if (!_bitmapCache.TryGetValue(CacheId!, out _bitmap))
        {
            CreateBitmap();
            _bitmapCache[CacheId] = _bitmap;
        }

        base.OnInitialized();
    }

    private void OnPaintSurface(SKPaintSurfaceEventArgs args)
    {
        SKCanvas canvas = args.Surface.Canvas;
        canvas.Clear();
        args.DrawLabels(MinX, MinY, MaxX, MaxY, LabelX, LabelY, ThemeService.Scheme);
        canvas.DrawBitmap(_bitmap, new SKRect(20,20, 420, 220));
        canvas.DrawCircle((float)(_seedX * 400 + 20), (float)(200 - _seedY * 200 + 20), 6, _seedPaint);
    }

    private void OnClick(MouseEventArgs args)
    {
        if (!(args.OffsetX is > 20 and < 420 
              && args.OffsetY is > 20 and < 220))
            return;
        _seedX = (args.OffsetX - 20) / 400;
        _seedY = (200 - (args.OffsetY - 20)) / 200;
        uint color = GetColor(_seedX, _seedY);
        ThemeService.Seed = color;
        _view?.Invalidate();
    }

    protected override void SetFromSeed(uint seed)
    {
        if (FuncInverse == null) return;
        (double x, double y) = FuncInverse(seed);
        _seedX = (x - MinX) / (MaxX - MinX);
        _seedY = (y - MinY) / (MaxY - MinY);
        _view?.Invalidate();
    }

    private uint GetColor(double x, double y)
        => Func(
            x * (MaxX - MinX) + MinX,
            y * (MaxY - MinY) + MinY
        );
    
    private unsafe void CreateBitmap()
    {
        _bitmap = new(Resolution, Resolution / 2, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        IntPtr data = _bitmap.GetPixels(out IntPtr length);
        Span<uint> span = new(data.ToPointer(), length.ToInt32());
        for (int y = 0; y < _bitmap.Height; y++)
        {
            for (int x = 0; x < _bitmap.Width; x++)
            {
                uint color = GetColor(x / 200d, 1 - y / 100d);
                span[x + y * _bitmap.Width] = color;
            }
        }
    }
}