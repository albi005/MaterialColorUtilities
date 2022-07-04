using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Utils;
using System.Runtime.Versioning;

namespace MaterialColorUtilities.Maui;

public partial class DynamicColorService
{
    private int prevSeedSource = -1;
    private readonly WallpaperManager wallpaperManager = WallpaperManager.GetInstance(Platform.AppContext);

    partial void PlatformInitialize()
    {
        if (wallpaperManager == null) return;
        try
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(27))
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(31))
                    SetFromAndroid12AccentColors();
                else
                {
                    SetFromAndroid8PrimaryWallpaperColor();
                    wallpaperManager.ColorsChanged += (sender, args) =>
                    {
                        if (args.Which == (int)WallpaperManagerFlags.Lock) return;

#pragma warning disable CA1416
                        MainThread.BeginInvokeOnMainThread(
                            SetFromAndroid8PrimaryWallpaperColor);
#pragma warning restore CA1416
                    };
                }

            }
            else
            {
                TrySetFromQuantizedWallpaperColors();
            }
        }
        catch { }
    }

    [SupportedOSPlatform("android31.0")]
    public void SetFromAndroid12AccentColors()
    {
        // We have access to the basic tones like 0, 10, 20 etc. of every tonal palette,
        // but if a different tone is required, we need access to the seed color.
        // Android doesn't seem to expose the seed color, so we have to get creative to get it.

        // We will use the tone of the primary color with the highest chroma as the seed,
        // because it should have the same hue, and chroma will be close enough.
        int[] primaryIds =
        {
            Android.Resource.Color.SystemAccent1500,
            Android.Resource.Color.SystemAccent110,
            Android.Resource.Color.SystemAccent150,
            Android.Resource.Color.SystemAccent1100,
            Android.Resource.Color.SystemAccent1200,
            Android.Resource.Color.SystemAccent1300,
            Android.Resource.Color.SystemAccent1400,
            Android.Resource.Color.SystemAccent1600,
            Android.Resource.Color.SystemAccent1700,
            Android.Resource.Color.SystemAccent1800,
            Android.Resource.Color.SystemAccent1900,
        };
        double maxChroma = -1;
        int closestColor = 0;
        foreach (int id in primaryIds)
        {
            int color = Platform.AppContext.Resources.GetColor(id, null);

            if (id == Android.Resource.Color.SystemAccent1500)
            {
                // If Primary50 didn't change, return
                if (color == prevSeedSource) return;
                prevSeedSource = color;
            }

            Hct hct = Hct.FromInt(color);
            if (hct.Chroma > maxChroma)
            {
                maxChroma = hct.Chroma;
                closestColor = color;
            }
        }

        SetSeed(closestColor);
    }

    [SupportedOSPlatform("android27.0")]
    private void SetFromAndroid8PrimaryWallpaperColor()
    {
        int color = wallpaperManager.GetWallpaperColors((int)WallpaperManagerFlags.System).PrimaryColor.ToArgb();
        SetSeed(color);
    }

    private async void TrySetFromQuantizedWallpaperColors()
    {
        List<int> colors = await Task.Run(async () =>
        {
            int[] pixels = await GetWallpaperPixels();
            if (pixels == null) return null;
            return ImageUtils.ColorsFromImage(pixels);
        });
        if (colors == null) return;

        SetSeed(colors.First());
    }

    private async Task<int[]> GetWallpaperPixels()
    {
        if (wallpaperManager == null) return null;

        if (OperatingSystem.IsAndroidVersionAtLeast(24))
        {
            int wallpaperId = wallpaperManager.GetWallpaperId(WallpaperManagerFlags.System);
            if (prevSeedSource == wallpaperId) return null;
            prevSeedSource = wallpaperId;
        }

        // Need permission to read wallpaper
        if ((await Permissions.CheckStatusAsync<Permissions.StorageRead>()) != PermissionStatus.Granted)
            return null;

        Drawable drawable = wallpaperManager.Drawable;
        if (drawable is not BitmapDrawable bitmapDrawable) return null;
        Bitmap bitmap = Bitmap.CreateScaledBitmap(bitmapDrawable.Bitmap, 112, 112, false);
        int[] pixels = new int[bitmap.ByteCount / 4];
        bitmap.GetPixels(pixels, 0, bitmap.Width, 0, 0, bitmap.Width, bitmap.Height);

        return pixels;
    }
}
