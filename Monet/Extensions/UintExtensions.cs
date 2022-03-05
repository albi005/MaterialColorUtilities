using System.Drawing;

namespace MaterialColorUtilities.Extensions
{
    public static class UintExtensions
    {
        public static Color ToColor(this uint argb) => Color.FromArgb((int)argb);
    }
}
