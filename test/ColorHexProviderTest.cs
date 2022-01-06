using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shifter.Providers;

namespace ShifterTest
{
    [TestClass]
    public class ColorHexProviderTest
    {
        private ColorHexProvider _provider;

        [TestInitialize]
        public void Setup()
        {
            _provider = new ColorHexProvider();
        }

        [DataTestMethod]
        [DataRow("#ffffff", "#f2f2f2", "#ffffff")]
        [DataRow("#FFFFFF", "#F2F2F2", "#FFFFFF")]
        [DataRow("#ffff00", "#f2f200", "#ffff0c")]
        [DataRow("#ff0", "#f2f200", "#ffff0c")]
        [DataRow("#000000", "#000000", "#0c0c0c")]
        public void HexColor(string textIn, string down, string up)
        {
            bool successDown = _provider.TryShiftLine(textIn, textIn.Length, ShiftDirection.Down, out ShiftResult resultDown);
            bool successUp = _provider.TryShiftLine(textIn, textIn.Length, ShiftDirection.Up, out ShiftResult resultUp);

            Assert.IsTrue(successDown);
            Assert.IsTrue(successUp);
            Assert.AreEqual(down, resultDown.ShiftedText);
            Assert.AreEqual(up, resultUp.ShiftedText);
        }

        [DataTestMethod]
        [DataRow("#fffff")]
        [DataRow("#fffffff")]
        [DataRow("#ttrrww")]
        [DataRow("#aaaa")]
        [DataRow("ffffff")]
        [DataRow("fff")]
        public void InvalidHexColor(string textIn)
        {
            bool successDown = _provider.TryShiftLine(textIn, textIn.Length, ShiftDirection.Down, out _);
            bool successUp = _provider.TryShiftLine(textIn, textIn.Length, ShiftDirection.Up, out _);

            Assert.IsFalse(successDown);
            Assert.IsFalse(successUp);
        }
    }
}
