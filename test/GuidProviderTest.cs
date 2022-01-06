using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shifter.Providers;

namespace ShifterTest
{
    [TestClass]
    public class GuidProviderTest
    {
        private GuidProvider _provider;

        [TestInitialize]
        public void Setup()
        {
            _provider = new GuidProvider();
        }

        [DataTestMethod]
        [DataRow("df3fd217-9653-46a1-b3f2-b887bc0751dd", "df3fd217-9653-46a1-b3f2-b887bc0751dc", "df3fd217-9653-46a1-b3f2-b887bc0751de")]
        [DataRow("{df3fd217-9653-46a1-b3f2-b887bc0751dd}", "df3fd217-9653-46a1-b3f2-b887bc0751dc", "df3fd217-9653-46a1-b3f2-b887bc0751de")]
        [DataRow("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-0000000000ff", "00000000-0000-0000-0000-000000000001")]
        public void Guids(string textIn, string down, string up)
        {
            bool successDown = _provider.TryShiftLine(textIn, 5, ShiftDirection.Down, out ShiftResult resultDown);
            bool successUp = _provider.TryShiftLine(textIn, 5, ShiftDirection.Up, out ShiftResult resultUp);

            Assert.IsTrue(successDown);
            Assert.IsTrue(successUp);
            Assert.AreEqual(down, resultDown.ShiftedText);
            Assert.AreEqual(up, resultUp.ShiftedText);
        }

        [DataTestMethod]
        [DataRow("df3fd217-9653-46a1-b3f2-b887bc0751d")] // too short
        [DataRow("df3fd217-9653-46a1-b887bc0751dd")]
        [DataRow("df3fd217-9653-46a1--b887bc0751dd")]
        public void InvalidGuids(string textIn)
        {
            bool successDown = _provider.TryShiftLine(textIn, 5, ShiftDirection.Down, out _);

            Assert.IsFalse(successDown);
        }
    }
}
