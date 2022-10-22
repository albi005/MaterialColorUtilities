namespace MaterialColorUtilities.Utils
{
    /// <summary>
    /// Utility methods for string representations of colors.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Hex string representing color, ex. #0000FF for blue.
        /// </summary>
        /// <param name="argb">ARGB representation of a color.</param>
        public static string HexFromArgb(uint argb) => "#" + argb.ToString("X6").Substring(2);
    }
}
