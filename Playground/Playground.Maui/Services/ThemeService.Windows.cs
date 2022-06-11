using Microsoft.Win32;
using SkiaSharp;

namespace Playground.Maui.Services;

public partial class ThemeService
{
    private string prevPath;

    private Task<int[]> GetWallpaperPixels()
    {
        string path = GetWallpaperPath();
        if (path == prevPath || string.IsNullOrWhiteSpace(path)) return Task.FromResult<int[]>(null);
        prevPath = path;

        using FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
        SKBitmap bitmap = SKBitmap.Decode(stream).Resize(new SKImageInfo(112, 112), SKFilterQuality.Medium);

        int[] pixels = bitmap.Pixels.Select(p => (int)(uint)p).ToArray();
        return Task.FromResult(pixels);
    }

    private static string GetWallpaperPath()
    {
        using RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
        if (regKey == null) return null;
        var path = regKey.GetValue("WallPaper").ToString();
        regKey.Close();
        return path;
    }
}
