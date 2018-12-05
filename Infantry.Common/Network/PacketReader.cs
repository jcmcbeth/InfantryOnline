namespace Infantry.Network
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    public class PacketReader
    {
        private readonly byte[] buffer;
        private int offset;

        public PacketReader(byte[] buffer)
        {
            this.buffer = buffer;
            this.offset = 0;
        }

        public byte[] ReadBytes(int count)
        {
            if (this.offset + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            byte[] values = new byte[count];

            Array.Copy(this.buffer, this.offset, values, 0, count);

            this.offset += count;

            return values;
        }

        public byte ReadByte()
        {
            return this.buffer[this.offset++];
        }

        public int ReadInt32()
        {
            int value = 0;

            if (BitConverter.IsLittleEndian)
            {
                value = (this.buffer[this.offset] << 24) |
                    (this.buffer[this.offset + 1] << 16) |
                    (this.buffer[this.offset + 2] << 8) |
                    this.buffer[this.offset + 3];
            }
            else
            {
                value = BitConverter.ToInt32(this.buffer, this.offset);
            }

            this.offset += sizeof(int);

            return value;
        }

        public ushort ReadUInt16()
        {
            ushort value = 0;

            if (BitConverter.IsLittleEndian)
            {
                value = (ushort)((this.buffer[this.offset] << 8) |
                    this.buffer[this.offset + 1]);
            }
            else
            {
                value = BitConverter.ToUInt16(this.buffer, this.offset);
            }

            this.offset += sizeof(short);

            return value;
        }

        public IPAddress ReadIPAddress()
        {
            var span = this.buffer.AsSpan(this.offset, 4);

            this.offset += 4;

            return new IPAddress(span);
        }

        public string ReadString(int count)
        {
            int length = 0;
            for (int i = 0; i < count; i++)
            {
                if (this.buffer[this.offset + i] == 0)
                {
                    length = i;
                    break;
                }
            }

            var value = Encoding.ASCII.GetString(this.buffer, this.offset, length);
            this.offset += count;

            return value;
        }

        public bool ReadBoolean()
        {
            byte value = this.buffer[this.offset++];

            return value == 1 ? true : false;
        }

        public string ReadString()
        {
            int count = 0;

            while (this.buffer[this.offset + count] != 0)
            {
                count++;
            }

            var value = Encoding.ASCII.GetString(this.buffer, this.offset, count);

            // Include the null terminator
            this.offset += count + 1;

            return value;
        }

        public short ReadInt16()
        {
            short value = 0;

            if (BitConverter.IsLittleEndian)
            {
                value = (short)((this.buffer[this.offset] << 8) |
                    this.buffer[this.offset + 1]);
            }
            else
            {
                value =  BitConverter.ToInt16(this.buffer, this.offset);
            }

            this.offset += sizeof(short);

            return value;
        }

        public short ReadLittleEndianInt16()
        {
            short value = 0;

            if (BitConverter.IsLittleEndian)
            {
                value = BitConverter.ToInt16(this.buffer, this.offset);
                
            }
            else
            {
                value = (short)((this.buffer[this.offset] << 8) |
                    this.buffer[this.offset + 1]);
            }

            this.offset += sizeof(short);

            return value;
        }

        public bool CanRead()
        {
            return this.offset < this.buffer.Length;
        }

        public void CopyBytes(byte[] destination, int index, int length)
        {
            Array.Copy(this.buffer, this.offset, destination, index, length);

            this.offset += length;
        }

        /// <summary>
        /// Copies all the remaining bytes from the buffer into a destination array.
        /// </summary>
        /// <param name="destination">The destination array to copy the bytes to.</param>
        /// <param name="index">The index in the destination array to start copying the bytes to.</param>
        /// <returns>Number of bytes copied.</returns>
        public int CopyBytes(byte[] destination, int index)
        {
            int length = this.buffer.Length - this.offset;

            this.CopyBytes(destination, index, length);

            return length;
        }
    }
}
