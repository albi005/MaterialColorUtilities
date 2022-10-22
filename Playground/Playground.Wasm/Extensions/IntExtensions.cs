using MaterialColorUtilities.Utils;
using MudBlazor.Utilities;

namespace Playground.Wasm.Extensions
{
    public static class IntExtensions
    {
        public static MudColor ToMudColor(this uint argb) => new(
            (int)ColorUtils.RedFromArgb(argb),
            (int)ColorUtils.GreenFromArgb(argb),
            (int)ColorUtils.BlueFromArgb(argb),
            (int)ColorUtils.AlphaFromArgb(argb));
    }
}
