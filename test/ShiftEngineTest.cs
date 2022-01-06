using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shifter.Providers;

namespace ShifterTest
{
    [TestClass]
    public class ShiftEngineTest
    {
        [DataTestMethod]
        [DataRow("123 true #fff blue", 5, "false", "false")]
        public void Numbers(string textIn, int position, string down, string up)
        {
            bool successDown = ShiftEngine.TryShift(textIn, position, ShiftDirection.Down, out ShiftResult resultDown);
            bool successUp = ShiftEngine.TryShift(textIn, position, ShiftDirection.Up, out ShiftResult resultUp);

            Assert.IsTrue(successDown);
            Assert.IsTrue(successUp);
            Assert.AreEqual(down, resultDown.ShiftedText);
            Assert.AreEqual(up, resultUp.ShiftedText);
        }
    }
}
