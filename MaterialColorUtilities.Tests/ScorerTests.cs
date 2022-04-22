using MaterialColorUtilities.Score;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MaterialColorUtilities.Tests
{
    [TestClass]
    public class ScorerTests
    {
        [TestMethod]
        public void PrioritizesChromaWhenProportionsAreEqual()
        {
            Dictionary<int, int> colorsToPopulation = new()
            {
                { unchecked((int)0xffff0000), 1 },
                { unchecked((int)0xff00ff00), 1 },
                { unchecked((int)0xff0000ff), 1 },
            };
            List<int> ranked = Scorer.Score(colorsToPopulation);

            Assert.AreEqual(unchecked((int)0xffff0000), ranked[0]);
            Assert.AreEqual(unchecked((int)0xff00ff00), ranked[1]);
            Assert.AreEqual(unchecked((int)0xff0000ff), ranked[2]);
        }

        [TestMethod]
        public void GeneratesGBlueWhenNoColorsAvailable()
        {
            Dictionary<int, int> colorsToPopulation = new()
            {
                { unchecked((int)0xff000000), 1 },
            };
            List<int> ranked = Scorer.Score(colorsToPopulation);

            Assert.AreEqual(unchecked((int)0xff4285F4), ranked[0]);
        }

        [TestMethod]
        public void DedupesNearbyHues()
        {
            Dictionary<int, int> colorsToPopulation = new()
            {
                { unchecked((int)0xff008772), 1 }, // H 180 C 42 T 50
                { unchecked((int)0xff318477), 1 }, // H 184 C 35 T 50
            };
            List<int> ranked = Scorer.Score(colorsToPopulation);

            Assert.AreEqual(1, ranked.Count);
            Assert.AreEqual(unchecked((int)0xff008772), ranked[0]);
        }

        // Not yet...
        //[TestMethod]
        //public void MaximizesTheHueDistance()
        //{
        //    Dictionary<int, int> colorsToPopulation = new()
        //    {
        //        { unchecked((int)0xff008772), 1 }, // H 180 C 42 T 50
        //        { unchecked((int)0xff008587), 1 }, // H 198 C 50 T 50
        //        { unchecked((int)0xff007EBC), 1 }, // H 245 C 50 T 50
        //    };
        //    List<int> ranked = Scorer.Score(colorsToPopulation);

        //    Assert.AreEqual(2, ranked.Count);
        //    Assert.AreEqual(unchecked((int)0xff007EBC), ranked[0]);
        //    Assert.AreEqual(unchecked((int)0xff008772), ranked[1]);
        //}
    }
}
