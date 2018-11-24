namespace Infantry.Tests
{
    using Infantry.Network;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    [TestClass]
    public class PacketReaderTests
    {
        [TestMethod]
        public void ReadInt32_BigEndianOrderBuffer_CorrectInt32Returned()
        {
            byte[] buffer = { 0x6C, 0x3D, 0x85, 0x7A };

            var target = new PacketReader(buffer);

            var actual = target.ReadInt32();

            Assert.AreEqual(1815971194, actual);
        }

        [TestMethod]
        public void ReadInt16_BigEndianOrderBuffer_CorrectInt16Returned()
        {
            byte[] buffer = { 0x05, 0x01 };

            var target = new PacketReader(buffer);

            var actual = target.ReadInt16();

            Assert.AreEqual(1281, actual);
        }

        [TestMethod]
        public void ReadIpAddress_ValidIPAddressInBuffer_IPAddressReturned()
        {
            byte[] buffer = { 108, 61, 133, 122 };

            var target = new PacketReader(buffer);

            var actual = target.ReadIPAddress();

            Assert.AreEqual(new IPAddress(buffer), actual);
        }

        [TestMethod]
        public void ReadString_int_ValidStringInBuffer_StringReturned()
        {
            byte[] buffer =
            {
                0x5B, 0x49, 0x3A, 0x48, 0x51, 0x5D, 0x20, 0x43, 0x6F, 0x6D,
                0x62, 0x69, 0x6E, 0x65, 0x64, 0x20, 0x41, 0x72, 0x6D, 0x73,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00
            };

            var target = new PacketReader(buffer);

            var actual = target.ReadString(32);

            Assert.AreEqual("[I:HQ] Combined Arms", actual);
        }

        [TestMethod]
        public void ReadString_ValidStringInBuffer_StringReturned()
        {
            byte[] buffer =
            {
                0x5B, 0x49, 0x3A, 0x48, 0x51, 0x5D, 0x20, 0x43, 0x6F, 0x6D,
                0x62, 0x69, 0x6E, 0x65, 0x64, 0x20, 0x41, 0x72, 0x6D, 0x73,
                0x00, 0x01, 0x02, 0x03
            };

            var target = new PacketReader(buffer);

            var actual = target.ReadString(32);

            Assert.AreEqual("[I:HQ] Combined Arms", actual);
        }

        [TestMethod]
        public void ReadBoolean_TrueBooleanInBuffer_TrueReturned()
        {
            byte[] buffer = { 1 };

            var target = new PacketReader(buffer);

            var actual = target.ReadBoolean();

            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void ReadBoolean_TrueBooleanInBuffer_FalseReturned()
        {
            byte[] buffer = { 0 };

            var target = new PacketReader(buffer);

            var actual = target.ReadBoolean();

            Assert.AreEqual(false, actual);
        }
    }
}
