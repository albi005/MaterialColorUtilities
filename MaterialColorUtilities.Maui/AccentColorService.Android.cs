﻿using System.Runtime.Versioning;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Utils;
using Microsoft.Maui.LifecycleEvents;
using Size = Android.Util.Size;

namespace MaterialColorUtilities.Maui;

public class AccentColorService : IAccentColorService
{
    private const string AccentColorCacheKey = "MaterialColorUtilities.Maui.AccentColorCache";
    private const string WallpaperIdKey = "MaterialColorUtilities.Maui.WallpaperId";

    private readonly LifecycleEventService _lifecycleEventService;
    private readonly IPreferences _preferences;
    private readonly WallpaperManager _wallpaperManager = WallpaperManager.GetInstance(Platform.AppContext);

    private bool _hasInitialized;
    private int? _wallpaperId;
    private int? _accentColorCache;

    public AccentColorService(ILifecycleEventService lifecycleEventService, IPreferences preferences)
    {
        _lifecycleEventService = (LifecycleEventService)lifecycleEventService;
        _preferences = preferences;
    }

    private void EnsureInitialized()
    {
        if (_hasInitialized) return;
        _hasInitialized = true;

        if (OperatingSystem.IsAndroidVersionAtLeast(27))
        {
            _lifecycleEventService.AddAndroid(a => a.OnResume(_ => OnAccentColorChanged?.Invoke()));
        }
        else if (OperatingSystem.IsAndroidVersionAtLeast(24))
        {
            if (_preferences.ContainsKey(AccentColorCacheKey))
            {
                _wallpaperId = _preferences.Get(WallpaperIdKey, 0);
                _accentColorCache = _preferences.Get(AccentColorCacheKey, 0);
            }

            _lifecycleEventService.AddAndroid(a =>
            {
#pragma warning disable CA1416
                a.OnResume(_ => CheckWallpaper());
#pragma warning restore CA1416

                a.OnRequestPermissionsResult((_, _, permissions, results) =>
                {
                    if (results.All(x => x == Permission.Granted)
                        && permissions.Contains(Manifest.Permission.ReadExternalStorage))
#pragma warning disable CA1416
                        CheckWallpaper();
#pragma warning restore CA1416
                });
            });
        }
    }

    public int? AccentColor => Environment.OSVersion.Version.Major switch
    {
#pragma warning disable CA1416
        >= 31 => GuessAndroid12Seed(),
        >= 27 => GetAndroid8PrimaryWallpaperColor(),
#pragma warning restore CA1416
        >= 24 => _accentColorCache,
        _ => null
    };

    private event Action OnAccentColorChanged;

    event Action IAccentColorService.OnAccentColorChanged
    {
        add
        {
            EnsureInitialized();
            OnAccentColorChanged += value;
            if (OperatingSystem.IsAndroidVersionAtLeast(24) && !OperatingSystem.IsAndroidVersionAtLeast(27))
                CheckWallpaper();
        }
        remove => OnAccentColorChanged -= value;
    }

    [SupportedOSPlatform("android31.0")]
    private int? GuessAndroid12Seed()
    {
        // We have access to the basic tones like 0, 10, 20 etc. of every tonal palette,
        // but if a different tone is required, we need access to the seed color.
        // Android doesn't seem to expose the seed color, so we have to get creative to get it.

        // We will use the tone of the primary color with the highest chroma as the seed,
        // because it has the same hue as the actual seed and its chroma will be close enough.
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
            int color = Platform.AppContext.Resources!.GetColor(id, null);

            if (id == Android.Resource.Color.SystemAccent1500)
            {
                // If Primary50 didn't change, return
                if (color == _wallpaperId) return _accentColorCache;
                _wallpaperId = color;
            }

            Hct hct = Hct.FromInt(color);
            if (hct.Chroma > maxChroma)
            {
                maxChroma = hct.Chroma;
                closestColor = color;
            }
        }

        _accentColorCache = closestColor;
        return closestColor;
    }

    [SupportedOSPlatform("android27.0")]
    private int? GetAndroid8PrimaryWallpaperColor()
    {
        WallpaperColors colors = _wallpaperManager.GetWallpaperColors((int)WallpaperManagerFlags.System);
        return colors?.PrimaryColor.ToArgb();
    }

    // GetWallpaperId is only available from API 24, and without it we would have to quantize
    // the wallpaper everytime the app is resumed.
    [SupportedOSPlatform("android24.0")]
    private async void CheckWallpaper()
    {
        if (OnAccentColorChanged == null) return;

        // Need permission to read wallpaper
        if (await Permissions.CheckStatusAsync<Permissions.StorageRead>() != PermissionStatus.Granted)
            return;

        int wallpaperId = _wallpaperManager.GetWallpaperId(WallpaperManagerFlags.System);
        if (_wallpaperId == wallpaperId) return;

        _wallpaperId = wallpaperId;

        _accentColorCache = await Task.Run(QuantizeWallpaper);

        _preferences.Set(WallpaperIdKey, wallpaperId);

        if (_accentColorCache == null)
            _preferences.Remove(AccentColorCacheKey);
        else
            _preferences.Set(AccentColorCacheKey, (int)_accentColorCache);
        OnAccentColorChanged?.Invoke();
    }

    /// <summary>
    /// Compute a seed color using the algorithms included in MaterialColorUtilities
    /// </summary>
    /// <remarks>Requires permission <see cref="Permissions.StorageRead"/></remarks>
    private int? QuantizeWallpaper()
    {
        int[] pixels = GetWallpaperPixels();
        if (pixels == null) return null;
        return ImageUtils.ColorsFromImage(pixels)[0];
    }

    private int[] GetWallpaperPixels()
    {
        Drawable drawable = _wallpaperManager.Drawable;
        if (drawable is not BitmapDrawable bitmapDrawable || bitmapDrawable.Bitmap == null) return null;
        Bitmap bitmap = bitmapDrawable.Bitmap;
        if (bitmap.Height * bitmap.Width > 112 * 112)
        {
            Size optimalSize = CalculateOptimalSize(bitmap.Width, bitmap.Height);
            bitmap = Bitmap.CreateScaledBitmap(bitmap, optimalSize.Width, optimalSize.Height, false);
        }

        int[] pixels = new int[bitmap!.ByteCount / 4];
        bitmap.GetPixels(pixels, 0, bitmap.Width, 0, 0, bitmap.Width, bitmap.Height);

        return pixels;
    }

    // From https://cs.android.com/android/platform/superproject/+/384d0423f9e93790e76399a5291731f6cfea40e8:frameworks/base/core/java/android/app/WallpaperColors.java
    private static Size CalculateOptimalSize(int width, int height)
    {
        long area = width * height;
        if (area > 112 * 112)
        {
            double scale = Math.Sqrt(112 * 112 / (double)area);
            width = Math.Max((int)(width * scale), 1);
            height = Math.Max((int)(height * scale), 1);
        }

        return new(width, height);
    }
}