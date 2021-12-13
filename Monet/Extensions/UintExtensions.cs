using System.Drawing;

namespace Monet.Extensions
{
    public static class UintExtensions
    {
        public static Color ToColor(this uint argb) => Color.FromArgb((int)argb);
    }
}
