namespace Playground.Maui.Components;

public class ColorPlot : GraphicsView
{
    private readonly Func<double, double, int> _func;
    private readonly double _xMax;
    private readonly double _yMax;
    private readonly Color[,] _pixels = new Color[400, 200];

    public ColorPlot(Func<double, double, int> func, string xLabel, double xMax, string yLabel, double yMax)
    {
        _func = func;
        _xMax = xMax;
        _yMax = yMax;

        HeightRequest = 240;
        WidthRequest = 460;
        HorizontalOptions = LayoutOptions.Start;

        _ = Task.Run(Calculate).ContinueWith(t =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Drawable = new ColorPlotDrawable(_pixels, xLabel, xMax, yLabel, yMax);
            });
        });
    }

    private void Calculate()
    {
        //for (var y = 0; y < 200; y++)
        //{
        //    for (var x = 0; x < 400; x++)
        //    {
        //        _pixels[x, y] = Color.FromInt(_func(x / 400.0 * _xMax, y / 200.0 * _yMax));
        //    }
        //}
        Parallel.For(0, 200, y =>
        {
            for (var x = 0; x < 400; x++)
            {
                _pixels[x, y] = Color.FromInt(_func(x / 400.0 * _xMax, y / 200.0 * _yMax));
            }
        });
    }

    private class ColorPlotDrawable : IDrawable
    {
        private readonly Color[,] pixels;
        private readonly string xLabel;
        private readonly double xMax;
        private readonly string yLabel;
        private readonly double yMax;

        public ColorPlotDrawable(Color[,] pixels, string xLabel, double xMax, string yLabel, double yMax)
        {
            this.pixels = pixels;
            this.xLabel = xLabel;
            this.xMax = xMax;
            this.yLabel = yLabel;
            this.yMax = yMax;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            for (int x = 0; x < 400; x++)
            {
                for (int y = 0; y < 200; y++)
                {
                    canvas.StrokeColor = pixels[x, y];
                    canvas.DrawRectangle(x + 20, y + 20, 1, 1);
                }
            }
            canvas.FontColor = Colors.Gray;
            canvas.FontSize = 14;
            canvas.DrawString(xLabel, 220, 14, HorizontalAlignment.Center);
            canvas.DrawString(xMax.ToString(), 424, 14, HorizontalAlignment.Left);
            canvas.DrawString(yMax.ToString(), 2, 234, HorizontalAlignment.Left);
            canvas.Rotate(-90);
            canvas.DrawString(yLabel, -120, 12, HorizontalAlignment.Center);
        }
    }
}
