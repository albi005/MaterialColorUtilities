using System.Runtime.Versioning;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Utils;
using Microsoft.Maui.LifecycleEvents;
using Size = Android.Util.Size;
using R = Android.Resource.Color;

namespace MaterialColorUtilities.Maui;

public class DynamicColorService : IDynamicColorService
{
    private const string SeedColorCacheKey = "MaterialColorUtilities.Maui.SeedColorCache";
    private const string WallpaperIdKey = "MaterialColorUtilities.Maui.WallpaperId";

    private readonly LifecycleEventService _lifecycleEventService;
    private readonly IPreferences _preferences;
    private readonly WallpaperManager _wallpaperManager = WallpaperManager.GetInstance(Platform.AppContext)!;

    private bool _hasInitialized;
    private int? _wallpaperId;

    private uint? _prevPrimary60;
    private uint? _prevSecondary60;
    private uint? _prevTertiary60;

    public DynamicColorService(ILifecycleEventService lifecycleEventService, IPreferences preferences)
    {
        _lifecycleEventService = (LifecycleEventService)lifecycleEventService;
        _preferences = preferences;
    }

    private void EnsureInitialized()
    {
        if (_hasInitialized) return;
        _hasInitialized = true;

        if (_preferences.ContainsKey(SeedColorCacheKey))
        {
            _wallpaperId = _preferences.Get(WallpaperIdKey, 0);
            SeedColor = _preferences.Get(SeedColorCacheKey, 0U);
        }

        _lifecycleEventService.AddAndroid(a =>
        {
            a.OnResume(_ => Update());

            if (OperatingSystem.IsAndroidVersionAtLeast(24) && !OperatingSystem.IsAndroidVersionAtLeast(27))
            {
                a.OnRequestPermissionsResult((_, _, permissions, results) =>
                {
                    if (results.All(x => x == Permission.Granted)
                        && permissions.Contains(Manifest.Permission.ReadExternalStorage))
                        Update();
                });
            }
        });
        
        Update();
    }

    public CorePalette? CorePalette { get; } = OperatingSystem.IsAndroidVersionAtLeast(31) ? new() : null;

    public uint? SeedColor { get; private set; }

    private event Action? Changed;

    event Action IDynamicColorService.Changed
    {
        add
        {
            EnsureInitialized();
            Changed += value;
        }
        remove => Changed -= value;
    }

    private void Update()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
            CheckSystemColors();
        else if (OperatingSystem.IsAndroidVersionAtLeast(27))
            CheckWallpaperColors();
        else if (OperatingSystem.IsAndroidVersionAtLeast(24))
            CheckWallpaper();
    }

    [SupportedOSPlatform("android31.0")]
    private void CheckSystemColors()
    {
        if (_prevPrimary60 == GetColor(R.SystemAccent1400)
            && _prevSecondary60 == GetColor(R.SystemAccent2400)
            && _prevTertiary60 == GetColor(R.SystemAccent3400))
            return;
        _prevPrimary60 = GetColor(R.SystemAccent1400);
        _prevSecondary60 = GetColor(R.SystemAccent2400);
        _prevTertiary60 = GetColor(R.SystemAccent3400);

        Hct primaryHct = FindHighestChroma(R.SystemAccent150);
        SeedColor = primaryHct.ToInt();
        CorePalette!.Primary = TonalPalette.FromHueAndChroma(primaryHct.Hue, primaryHct.Chroma);
        CorePalette.Secondary = GuessTonalPalette(R.SystemAccent250);
        CorePalette.Tertiary = GuessTonalPalette(R.SystemAccent350);
        CorePalette.Neutral = GuessTonalPalette(R.SystemNeutral150);
        CorePalette.NeutralVariant = GuessTonalPalette(R.SystemNeutral250);

        Changed?.Invoke();
    }

    [SupportedOSPlatform("android27.0")]
    private void CheckWallpaperColors()
    {
        WallpaperColors? colors = _wallpaperManager.GetWallpaperColors((int)WallpaperManagerFlags.System);
        uint? seed = (uint?)colors?.PrimaryColor.ToArgb();
        if (seed == SeedColor) return;

        SeedColor = seed;
        Changed?.Invoke();
    }

    // GetWallpaperId is only available from API 24, and without it we would have to quantize
    // the wallpaper everytime the app is resumed.
    [SupportedOSPlatform("android24.0")]
    private async void CheckWallpaper()
    {
        if (Changed == null) return;

        // Need permission to read wallpaper
        if (await Permissions.CheckStatusAsync<Permissions.StorageRead>() != PermissionStatus.Granted)
            return;

        int wallpaperId = _wallpaperManager.GetWallpaperId(WallpaperManagerFlags.System);
        if (_wallpaperId == wallpaperId) return;

        _wallpaperId = wallpaperId;

        SeedColor = await Task.Run(QuantizeWallpaper);

        _preferences.Set(WallpaperIdKey, wallpaperId);

        if (SeedColor == null)
            _preferences.Remove(SeedColorCacheKey);
        else
            _preferences.Set(SeedColorCacheKey, (int)SeedColor);
        Changed?.Invoke();
    }

    /// <summary>
    /// Computes a seed color using the algorithms included in MaterialColorUtilities
    /// </summary>
    /// <remarks>Requires permission <see cref="Permissions.StorageRead"/></remarks>
    private uint? QuantizeWallpaper()
    {
        uint[]? pixels = GetWallpaperPixels();
        if (pixels == null) return null;
        return ImageUtils.ColorsFromImage(pixels)[0];
    }

    private uint[]? GetWallpaperPixels()
    {
        Drawable? drawable = _wallpaperManager.Drawable;
        if (drawable is not BitmapDrawable bitmapDrawable || bitmapDrawable.Bitmap == null) return null;
        Bitmap bitmap = bitmapDrawable.Bitmap;
        if (bitmap.Height * bitmap.Width > 112 * 112)
        {
            Size optimalSize = CalculateOptimalSize(bitmap.Width, bitmap.Height);
            bitmap = Bitmap.CreateScaledBitmap(bitmap, optimalSize.Width, optimalSize.Height, false)!;
        }

        int[] pixels = new int[bitmap!.ByteCount / 4];
        bitmap.GetPixels(pixels, 0, bitmap.Width, 0, 0, bitmap.Width, bitmap.Height);

        return (uint[])(object)pixels;
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

    [SupportedOSPlatform("android31.0")]
    private static TonalPalette GuessTonalPalette(int tone95Id)
    {
        Hct hct = FindHighestChroma(tone95Id);
        return TonalPalette.FromHueAndChroma(hct.Hue, hct.Chroma);
    }

    [SupportedOSPlatform("android31.0")]
    private static Hct FindHighestChroma(int tone95Id)
    {
        Hct result = Hct.FromInt(GetColor(tone95Id));
        for (int i = 1; i < 8; i++)
        {
            uint color = GetColor(tone95Id + i);
            Hct current = Hct.FromInt(color);
            if (current.Chroma > result.Chroma)
                result = current;
        }
        return result;
    }

    [SupportedOSPlatform("android31.0")]
    private static uint GetColor(int id) => (uint)(int)Platform.AppContext.Resources!.GetColor(id, null);
}