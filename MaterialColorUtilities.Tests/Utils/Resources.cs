using System;
using SkiaSharp;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MaterialColorUtilities.Tests.Utils
{
    public class Resources
    {
        public static uint[] LoadImage(string nameAndExtension)
        {
            string resourceId = $"MaterialColorUtilities.Tests.Resources.{nameAndExtension}";
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceId)!;
            SKBitmap bitmap = SKBitmap.Decode(stream).Resize(new SKImageInfo(112, 112), SKFilterQuality.Low);
            return Array.ConvertAll(bitmap.Pixels, p => (uint)p);
        }
    }
}
