using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shifter.Providers;

namespace ShifterTest
{
    [TestClass]
    public class ShiftEngineTest
    {
        [DataTestMethod]
        [DataRow("123 true #fff blue", 2, "122", "124")]
        public void Numbers(string textIn, int position, string down, string up)
        {
            bool successDown = ShiftEngine.TryShift(textIn, position, ShiftDirection.Down, null, out ShiftResult resultDown);
            bool successUp = ShiftEngine.TryShift(textIn, position, ShiftDirection.Up, null, out ShiftResult resultUp);

            Assert.IsTrue(successDown);
            Assert.IsTrue(successUp);
            Assert.AreEqual(down, resultDown.ShiftedText);
            Assert.AreEqual(up, resultUp.ShiftedText);
        }

        [DataTestMethod]
        [DataRow("123", 2, 0, "122", "124")]
        [DataRow("123", 2, 1, "121", "125")]
        [DataRow("123", 2, 99, "23", "223")]
        public void IncrementalNumbers(string textIn, int position, int sequence, string down, string up)
        {
            bool successDown = ShiftEngine.TryShift(textIn, position, ShiftDirection.Down, sequence, out ShiftResult resultDown);
            bool successUp = ShiftEngine.TryShift(textIn, position, ShiftDirection.Up, sequence, out ShiftResult resultUp);

            Assert.IsTrue(successDown);
            Assert.IsTrue(successUp);
            Assert.AreEqual(down, resultDown.ShiftedText);
            Assert.AreEqual(up, resultUp.ShiftedText);
        }
    }
}
