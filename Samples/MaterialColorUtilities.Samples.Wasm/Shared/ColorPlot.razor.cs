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
        private SKPaint paint = new() { Color = SKColors.White, Style = SKPaintStyle.Stroke, StrokeWidth = 2 };

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
            canvas.DrawBitmap(bitmap, 0, 0);
            canvas.DrawCircle((float)x, (float)y, 4, paint);
        }

        public void OnClick(MouseEventArgs args)
        {
            x = args.OffsetX;
            y = args.OffsetY;
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
