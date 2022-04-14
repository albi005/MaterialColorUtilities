namespace MaterialColorUtilities.Utils
{
    public class StringUtils
    {
        public static string HexFromArgb(int argb) => "#" + argb.ToString("X").Substring(2);
    }
}
