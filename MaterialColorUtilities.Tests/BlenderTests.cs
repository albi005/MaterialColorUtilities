using MaterialColorUtilities.Blend;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class BlenderTests
    {
        const int Red = unchecked((int)0xffff0000);
        const int Blue = unchecked((int)0xff0000ff);
        const int Green = unchecked((int)0xff00ff00);
        const int Yellow = unchecked((int)0xffffff00);
        
        [TestMethod]
        [DataRow(Red, Blue, 0xffFB0054)]
        [DataRow(Red, Green, 0xffDA5400)]
        [DataRow(Red, Yellow, 0xffDA5400)]
        [DataRow(Blue, Green, 0xff0047A7)]
        [DataRow(Blue, Red, 0xff5600DE)]
        [DataRow(Blue, Yellow, 0xff0047A7)]
        [DataRow(Green, Blue, 0xff00FC91)]
        [DataRow(Green, Red, 0xffADF000)]
        [DataRow(Green, Yellow, 0xffADF000)]
        [DataRow(Yellow, Blue, 0xffEBFFB2)]
        [DataRow(Yellow, Green, 0xffEBFFB2)]
        [DataRow(Yellow, Red, 0xffFFF6DC)]
        public void Harmonize(int designColor, int sourceColor, uint result)
        {
            int answer = Blender.Harmonize(designColor, sourceColor);
            Assert.AreEqual((int)result, answer);
        }
    }
}
