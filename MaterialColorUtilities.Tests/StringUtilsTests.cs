using MaterialColorUtilities.Utils;

namespace MaterialColorUtilities.Tests;

[TestClass]
public class StringUtilsTests
{
    [TestMethod]
    [DataRow(0x00000000U, "#000000")]
    [DataRow(0x000000FFU, "#0000FF")]
    [DataRow(0xFF0000FFU, "#0000FF")]
    [DataRow(0x0000FF00U, "#00FF00")]
    [DataRow(0x00FF0000U, "#FF0000")]
    [DataRow(0x00FFFFFFU, "#FFFFFF")]
    [DataRow(0xFFFFFFFFU, "#FFFFFF")]
    public void HexFromArgb(uint argb, string expected)
    {
        string actual = StringUtils.HexFromArgb(argb);
        Assert.AreEqual(expected, actual);
    }
}