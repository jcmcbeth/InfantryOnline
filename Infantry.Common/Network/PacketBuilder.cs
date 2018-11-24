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

            buffer.AddRange(buffer);
        }

        public byte[] GetBytes()
        {
            return buffer.ToArray();
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
