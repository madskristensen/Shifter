﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shifter.Providers;

namespace ShifterTest
{
    [TestClass]
    public class BooleanProviderTest
    {
        private BooleanProvider _provider;

        [TestInitialize]
        public void Setup()
        {
            _provider = new BooleanProvider();
        }

        [DataTestMethod]
        [DataRow("abc true number", 5, "false", "false")]
        [DataRow("abc False number", 5, "True", "True")]
        [DataRow("abc ON number", 5, "OFF", "OFF")]
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
