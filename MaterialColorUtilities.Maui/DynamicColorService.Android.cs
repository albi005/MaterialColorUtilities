using Android.App;
using Android.Graphics.Drawables;
using Android.Graphics;
using MaterialColorUtilities.Utils;
using System.Runtime.Versioning;

namespace MaterialColorUtilities.Maui;

public partial class DynamicColorService
{
    private int prevWallpaperId = -1;
    private readonly WallpaperManager wallpaperManager = WallpaperManager.GetInstance(Platform.AppContext);

    partial void PlatformInitialize()
    {
        if (wallpaperManager == null) return;
        try
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(27))
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(31))
                    SetFromAndroid12AccentColor();
                else
                    SetFromAndroid8PrimaryWallpaperColor();

                wallpaperManager.ColorsChanged += (sender, args) =>
                {
                    if (args.Which == (int)WallpaperManagerFlags.Lock) return;

                    // version checker complains without this
                    if (!OperatingSystem.IsAndroidVersionAtLeast(27)) return;

                    MainThread.BeginInvokeOnMainThread(
                        OperatingSystem.IsAndroidVersionAtLeast(31)
                        ? SetFromAndroid12AccentColor
                        : SetFromAndroid8PrimaryWallpaperColor);
                };
            }
            else
            {
                SetFromQuantizedWallpaperColors();
            }
        }
        catch { }
    }

    [SupportedOSPlatform("android31.0")]
    private void SetFromAndroid12AccentColor()
    {
        int color = Platform.AppContext.Resources.GetColor(Android.Resource.Color.SystemAccent1500, null);
        SetSeed(color);
    }

    [SupportedOSPlatform("android27.0")]
    private void SetFromAndroid8PrimaryWallpaperColor()
    {
        int color = wallpaperManager.GetWallpaperColors((int)WallpaperManagerFlags.System).PrimaryColor.ToArgb();
        SetSeed(color);
    }

    private async void SetFromQuantizedWallpaperColors()
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
            if (prevWallpaperId == wallpaperId) return null;
            prevWallpaperId = wallpaperId;
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
