using MaterialColorUtilities.Utils;
using MudBlazor.Utilities;

namespace MaterialColorUtilities.Samples.Wasm.Extensions
{
    public static class IntExtensions
    {
        public static MudColor ToMudColor(this int argb) => new(
            ColorUtils.RedFromArgb(argb),
            ColorUtils.GreenFromArgb(argb),
            ColorUtils.BlueFromArgb(argb),
            ColorUtils.AlphaFromArgb(argb));
    }
}
