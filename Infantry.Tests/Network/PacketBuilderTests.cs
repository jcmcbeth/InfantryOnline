namespace Infantry.Tests
{
    using Infantry.Network;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Text;

    [TestClass]
    public class PacketBuilderTests
    {
        private PacketBuilder target;

        [TestInitialize]
        public void Initialize()
        {
            target = new PacketBuilder();
        }

        [TestMethod]
        public void AddInt16_ValidInt16_BigEndianOrderingUsed()
        {
            target.AddInt16(0x0003);

            var actual = this.target.GetBytes();

            Assert.AreEqual(0, actual[0]);
            Assert.AreEqual(3, actual[1]);
        }

        [TestMethod]
        public void AddInt32_ValidInt32_BigEndianOrderingUsed()
        {
            target.AddInt32(0x01020304);

            var actual = this.target.GetBytes();

            Assert.AreEqual(1, actual[0]);
            Assert.AreEqual(2, actual[1]);
            Assert.AreEqual(3, actual[2]);
            Assert.AreEqual(4, actual[3]);
        }
    }
}
