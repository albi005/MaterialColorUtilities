using MaterialColorUtilities.Schemes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class SchemeTests
    {
        [TestMethod]
        public void BlueLight()
        {
            Scheme<int> scheme = new LightSchemeMapper().Map(new(unchecked((int)0xff0000ff)));
            Assert.AreEqual(unchecked((int)0xff333CFF), scheme.Primary);
        }

        [TestMethod]
        public void BlueDark()
        {
            Scheme<int> scheme = new DarkSchemeMapper().Map(new(unchecked((int)0xff0000ff)));
            Assert.AreEqual(unchecked((int)0xffBDC2FF), scheme.Primary);
        }

        [TestMethod]
        public void PurpleishLight()
        {
            Scheme<int> scheme = new LightSchemeMapper().Map(new(unchecked((int)0xff6750A4)));
            Assert.AreEqual(unchecked((int)0xff6750A4), scheme.Primary);
            Assert.AreEqual(unchecked((int)0xff625B71), scheme.Secondary);
            Assert.AreEqual(unchecked((int)0xff7D5260), scheme.Tertiary);
            Assert.AreEqual(unchecked((int)0xfffffbfe), scheme.Surface);
            Assert.AreEqual(unchecked((int)0xff1C1B1E), scheme.OnSurface);
        }

        [TestMethod]
        public void PurpleishDark()
        {
            Scheme<int> scheme = new DarkSchemeMapper().Map(new(unchecked((int)0xff6750A4)));
            Assert.AreEqual(unchecked((int)0xffd0bcff), scheme.Primary);
            Assert.AreEqual(unchecked((int)0xffCBC2DB), scheme.Secondary);
            Assert.AreEqual(unchecked((int)0xffEFB8C8), scheme.Tertiary);
            Assert.AreEqual(unchecked((int)0xff1c1b1e), scheme.Surface);
            Assert.AreEqual(unchecked((int)0xffE6E1E5), scheme.OnSurface);
        }
    }
}
