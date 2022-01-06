using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shifter.Providers;

namespace ShifterTest
{
    [TestClass]
    public class NumberProviderTest
    {
        private NumberProvider _provider;

        [TestInitialize]
        public void Setup()
        {
            _provider = new NumberProvider();
        }

        [DataTestMethod]
        [DataRow("abc 0.123 number", 5, "0.122", "0.124")]
        [DataRow("abc .123 number", 5, ".122", ".124")]
        [DataRow("123", 3, "122", "124")]
        [DataRow("00001", 3, "00000", "00002")]
        [DataRow("a:123;", 3, "122", "124")]
        [DataRow("a: 123;", 4, "122", "124")]
        [DataRow("\"123\"", 4, "122", "124")]
        [DataRow("\"123.45\"", 4, "123.44", "123.46")]
        [DataRow("\"-0.1\"", 4, "-0.2", "0.0")]
        [DataRow("\"+0.1\"", 4, "0.0", "0.2")]
        public void Numbers(string textIn, int position, string down, string up)
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
