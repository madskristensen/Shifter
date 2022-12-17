using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shifter.Providers;

namespace ShifterTest
{
    [TestClass]
    public class ColorNamesProviderTest
    {
        private ColorNamesProvider _provider;

        [TestInitialize]
        public void Setup()
        {
            _provider = new ColorNamesProvider();
        }

        [DataTestMethod]
        [DataRow("abc blue number", 6, "mediumblue", "royalblue")]
        [DataRow("abc Azure number", 6, "AliceBlue", "MintCream")]
        public void ColorNames(string textIn, int position, string down, string up)
        {
            bool successDown = _provider.TryShiftLine(textIn, position, ShiftDirection.Down, out ShiftResult resultDown);
            bool successUp = _provider.TryShiftLine(textIn, position, ShiftDirection.Up, out ShiftResult resultUp);

            Assert.IsTrue(successDown);
            Assert.IsTrue(successUp);
            Assert.AreEqual(down, resultDown.ShiftedText);
            Assert.AreEqual(up, resultUp.ShiftedText);
        }
    }
}
