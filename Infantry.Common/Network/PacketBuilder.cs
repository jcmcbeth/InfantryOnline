namespace Infantry.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class PacketBuilder
    {
        private List<byte> buffer;

        public PacketBuilder()
        {
            buffer = new List<byte>();
        }

        public void AddByte(byte data)
        {
            buffer.Add(data);
        }

        public void AddBytes(byte[] data)
        {
            buffer.AddRange(data);
        }

        public void AddInt32(int data)
        {
            var bytes = BitConverter.GetBytes(data);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            buffer.AddRange(bytes);
        }

        public void AddString(string text, int length)
        {
            var bytes = new byte[length];

            var textBytes = Encoding.ASCII.GetBytes(text);

            for (int i = 0; i < Math.Min(textBytes.Length, length); i++)
            {
                bytes[i] = textBytes[i];
            }

            buffer.AddRange(bytes);
        }

        public byte[] GetBytes()
        {
            return buffer.ToArray();
        }

        /// <summary>
        /// Adds the specified number of zero bytes to the packet.
        /// </summary>
        /// <param name="count">The number of zero bytes to add.</param>
        public void AddPadding(int count)
        {
            for (int i = 0; i < count; i++)
            {
                buffer.Add(0);
            }
        }

        public void AddInt16(short data)
        {
            var bytes = BitConverter.GetBytes(data);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            buffer.AddRange(bytes);
        }
    }
}
