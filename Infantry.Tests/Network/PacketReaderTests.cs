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
        public void ReadUInt16_BufferWithUnsignedInt16ThatUsesSignBit_UnsignedValueReturned()
        {
            byte[] buffer = { 0xF0, 0xFF };

            var target = new PacketReader(buffer);

            var actual = target.ReadUInt16();

            Assert.AreEqual((ushort)0xF0FF, actual);
        }

        [TestMethod]
        public void ReadIPAddress_ValidIPAddressInBuffer_IPAddressReturned()
        {
            byte[] buffer = { 108, 61, 133, 122 };

            var target = new PacketReader(buffer);

            var actual = target.ReadIPAddress();

            Assert.AreEqual(new IPAddress(buffer), actual);
        }

        [TestMethod]
        public void ReadIPAddress_ReadIPAddressAndFollowingInt16_CorrectInt16Returned()
        {
            byte[] buffer = { 108, 61, 133, 122, 0x0F, 0x00 };

            var target = new PacketReader(buffer);

            target.ReadIPAddress();
            var actual = target.ReadInt16();

            Assert.AreEqual(3840, actual);            
        }

        [TestMethod]
        public void ReadString_int_ValidStringInBuffer_StringReturned()
        {
            byte[] buffer =
            {
                0x5B, 0x49, 0x3A, 0x48, 0x51, 0x5D, 0x20, 0x43,
                0x6F, 0x6D, 0x62, 0x69, 0x6E, 0x65, 0x64, 0x20,
                0x41, 0x72, 0x6D, 0x73, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };

            var target = new PacketReader(buffer);

            var actual = target.ReadString(32);

            Assert.AreEqual("[I:HQ] Combined Arms", actual);
        }

        [TestMethod]
        public void ReadString_int_BufferWithPaddedStringAndByteValue_ByteValueReturned()
        {
            byte[] buffer =
            {
                0x5B, 0x49, 0x3A, 0x48, 0x51, 0x5D, 0x20, 0x43,
                0x6F, 0x6D, 0x62, 0x69, 0x6E, 0x65, 0x64, 0x20,
                0x41, 0x72, 0x6D, 0x73, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xFF
            };

            var target = new PacketReader(buffer);

            target.ReadString(32);

            var actual = target.ReadByte();

            Assert.AreEqual(0xFF, actual);
        }

        [TestMethod]
        public void ReadString_void_BufferWithPaddedStringAndByteValue_ByteValueReturned()
        {
            byte[] buffer =
            {
                0x5B, 0x49, 0x3A, 0x48, 0x51, 0x5D, 0x20, 0x43,
                0x6F, 0x6D, 0x62, 0x69, 0x6E, 0x65, 0x64, 0x20,
                0x41, 0x72, 0x6D, 0x73, 0x00,
                0xFF
            };

            var target = new PacketReader(buffer);

            target.ReadString();

            var actual = target.ReadByte();

            Assert.AreEqual(0xFF, actual);
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
        public void ReadBoolean_FalseBooleanInBuffer_FalseReturned()
        {
            byte[] buffer = { 0 };

            var target = new PacketReader(buffer);

            var actual = target.ReadBoolean();

            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void CanRead_NonEmptyBufferAndNoReadingDone_ReturnsTrue()
        {
            byte[] buffer = { 0 };

            var target = new PacketReader(buffer);

            var actual = target.CanRead();

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void CanRead_NonEmptyBufferAndReadingPerformed_ReturnsFalse()
        {
            byte[] buffer = { 0 };

            var target = new PacketReader(buffer);

            target.ReadByte();

            var actual = target.CanRead();

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void CopyBytes_array_int_NonEmptyBuffer_CanReadReturnsFalse()
        {
            // Arrange
            byte[] buffer = { 1, 2, 3, 4, 5 };

            var target = new PacketReader(buffer);

            // Act
            var destination = new byte[buffer.Length];
            target.CopyBytes(destination, 0);

            // Assert
            var actual = target.CanRead();

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void ReadInt16_BufferWithTwoInt16sAndReadTwice_SecondValueIsCorrect()
        {
            byte[] buffer = { 0x00, 0x00, 0x0F, 0x0A };

            var target = new PacketReader(buffer);

            target.ReadInt16();
            var actual = target.ReadInt16();

            Assert.AreEqual(3850, actual);
        }
    }
}
