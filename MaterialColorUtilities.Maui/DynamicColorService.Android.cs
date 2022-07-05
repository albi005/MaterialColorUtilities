using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Utils;
using Microsoft.Maui.LifecycleEvents;
using System.Runtime.Versioning;

namespace MaterialColorUtilities.Maui;

public partial class DynamicColorService
{
    private int _prevSeedSource = -1;
    private readonly WallpaperManager _wallpaperManager = WallpaperManager.GetInstance(Platform.AppContext);

    partial void PlatformInitialize()
    {
        if (!_options.UseDynamicColor) return;
        if (_wallpaperManager == null) return;
        try
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                SetFromAndroid12AccentColors();
                _lifecycleEventService.AddAndroid(android
                    => android.OnResume(_
                    => MainThread.BeginInvokeOnMainThread(
#pragma warning disable CA1416
                        SetFromAndroid12AccentColors
#pragma warning restore CA1416
                )));
            }
            else if (OperatingSystem.IsAndroidVersionAtLeast(27))
            {
                SetFromAndroid8PrimaryWallpaperColor();
                _wallpaperManager.ColorsChanged += (sender, args) =>
                {
                    if (args.Which == (int)WallpaperManagerFlags.Lock) return;

                    MainThread.BeginInvokeOnMainThread(
#pragma warning disable CA1416
                        SetFromAndroid8PrimaryWallpaperColor
#pragma warning restore CA1416
                    );
                };
            }
            else
                _ = TrySetFromQuantizedWallpaperColors();
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
                if (color == _prevSeedSource) return;
                _prevSeedSource = color;
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
    public void SetFromAndroid8PrimaryWallpaperColor()
    {
        int color = _wallpaperManager.GetWallpaperColors((int)WallpaperManagerFlags.System).PrimaryColor.ToArgb();
        SetSeed(color);
    }

    /// <summary>
    /// Tries to set colors by using Material Color Utilities to get colors from the system wallpaper.
    /// </summary>
    /// <returns><see langword="true"/> if the operation completed successfully, <see langword="false"/> otherwise.</returns>
    /// <remarks>Requires <see cref="Permissions.StorageRead"/></remarks>
    public async Task<bool> TrySetFromQuantizedWallpaperColors()
    {
        List<int> colors = await Task.Run(async () =>
        {
            int[] pixels = await GetWallpaperPixels();
            if (pixels == null) return null;
            return ImageUtils.ColorsFromImage(pixels);
        });
        if (colors == null) return false;

        SetSeed(colors.First());

        return true;
    }

    private async Task<int[]> GetWallpaperPixels()
    {
        if (_wallpaperManager == null) return null;

        if (OperatingSystem.IsAndroidVersionAtLeast(24))
        {
            int wallpaperId = _wallpaperManager.GetWallpaperId(WallpaperManagerFlags.System);
            if (_prevSeedSource == wallpaperId) return null;
            _prevSeedSource = wallpaperId;
        }

        // Need permission to read wallpaper
        if ((await Permissions.CheckStatusAsync<Permissions.StorageRead>()) != PermissionStatus.Granted)
            return null;

        Drawable drawable = _wallpaperManager.Drawable;
        if (drawable is not BitmapDrawable bitmapDrawable) return null;
        Bitmap bitmap = bitmapDrawable.Bitmap;
        if (bitmap.Height * bitmap.Width > 112 * 112)
        {
            Android.Util.Size optimalSize = CalculateOptimalSize(bitmap.Width, bitmap.Height);
            bitmap = Bitmap.CreateScaledBitmap(bitmap, optimalSize.Width, optimalSize.Height, false);
        }
        int[] pixels = new int[bitmap.ByteCount / 4];
        bitmap.GetPixels(pixels, 0, bitmap.Width, 0, 0, bitmap.Width, bitmap.Height);

        return pixels;
    }

    // From https://cs.android.com/android/platform/superproject/+/384d0423f9e93790e76399a5291731f6cfea40e8:frameworks/base/core/java/android/app/WallpaperColors.java
    private static Android.Util.Size CalculateOptimalSize(int width, int height)
    {
        int requestedArea = width * height;
        double scale = 1;
        if (requestedArea > 112 * 112)
            scale = Math.Sqrt(112 * 112 / (double)requestedArea);
        int newWidth = (int)(width * scale);
        int newHeight = (int)(height * scale);

        if (newWidth == 0)
            newWidth = 1;
        if (newHeight == 0)
            newHeight = 1;

        return new Android.Util.Size(newWidth, newHeight);
    }
}
