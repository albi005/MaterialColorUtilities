using Android.App;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace Playground.Maui.Services;

public partial class ThemeService
{
	private int prevWallpaperId = -1;

	private async Task<int[]> GetWallpaperPixels()
    {
		WallpaperManager wallpaperManager = WallpaperManager.GetInstance(Platform.AppContext);
		if (wallpaperManager == null) return null;

        int wallpaperId = wallpaperManager.GetWallpaperId(WallpaperManagerFlags.System);
        if (prevWallpaperId == wallpaperId) return null;
        prevWallpaperId = wallpaperId;

        if (!await RequestAccessToWallpaper()) return null;

		Drawable drawable = WallpaperManager.GetInstance(Platform.AppContext)?.Drawable;
		if (drawable is not BitmapDrawable bitmapDrawable) return null;
		Bitmap bitmap = Bitmap.CreateScaledBitmap(bitmapDrawable.Bitmap, 112, 112, false);
		int[] pixels = new int[bitmap.ByteCount / 4];
		bitmap.GetPixels(pixels, 0, bitmap.Width, 0, 0, bitmap.Width, bitmap.Height);

		return pixels;
	}

	private static async Task<bool> RequestAccessToWallpaper()
	{
		PermissionStatus permissionStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
		return permissionStatus == PermissionStatus.Granted
			|| ((permissionStatus == PermissionStatus.Unknown || permissionStatus == PermissionStatus.Denied)
			&& Platform.CurrentActivity != null
			&& await Permissions.RequestAsync<Permissions.StorageRead>() == PermissionStatus.Granted);
	}
}
