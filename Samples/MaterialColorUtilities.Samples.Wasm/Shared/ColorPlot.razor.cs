using MaterialColorUtilities.ColorAppearance;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace MaterialColorUtilities.Samples.Wasm.Shared
{
    public partial class ColorPlot
    {
        private SKBitmap bitmap;
        private SKCanvasView view;
        private double x;
        private double y;
        private readonly SKPaint paint = new() { Color = SKColors.White, Style = SKPaintStyle.Stroke, StrokeWidth = 2 };
        private readonly SKPaint textPaint = new() { Color = SKColors.Gray };

        /// <summary>
        /// The width of the bitmap that will be generated.
        /// </summary>
        [Parameter] public int Resolution { get; set; } = 400;

        protected override void OnInitialized()
        {
            bitmap = new(Resolution, Resolution / 2);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Hct hct = Hct.From(360 * x / bitmap.Width, 100, 100 * y / bitmap.Height);
                    bitmap.SetPixel(x, y, new SKColor((uint)hct.ToInt()));
                }
            }
            bitmap = bitmap.Resize(new SKImageInfo(400, 200), SKFilterQuality.None);
            base.OnInitialized();
        }

        public void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            var canvas = args.Surface.Canvas;
            canvas.DrawBitmap(bitmap, 20, 20);
            canvas.DrawCircle((float)x + 20, (float)y + 20, 6, paint);
            canvas.DrawText("0", 2, 10, textPaint);
            canvas.DrawText("Hue", 200, 10, textPaint);
            canvas.DrawText("360", 420, 10, textPaint);
            canvas.DrawText("100", 2, 234, textPaint);

            canvas.RotateDegrees(90);
            canvas.DrawText("Tone", 110, -2, textPaint);
        }

        public void OnClick(MouseEventArgs args)
        {
            if (!(20 < args.OffsetX
                && args.OffsetX < bitmap.Width + 20
                && 20 < args.OffsetY
                && args.OffsetY < bitmap.Height + 20))
                return;
            x = args.OffsetX - 20;
            y = args.OffsetY - 20;
            Hct hct = Hct.From(360 * x / bitmap.Width, 100, 100 * y / bitmap.Height);
            themeService.Seed = hct.ToInt();
        }

        protected override void SetFromSeed(int seed)
        {
            Hct hct = Hct.FromInt(seed);
            x = bitmap.Width * hct.Hue / 360;
            y = bitmap.Height * hct.Tone / 100;
            view?.Invalidate();
        }
    }
}
