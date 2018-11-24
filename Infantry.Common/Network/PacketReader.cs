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
            if (offset + count > buffer.Length)
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
            return this.buffer[offset++];
        }

        public int ReadInt32()
        {
            int value = 0;

            if (BitConverter.IsLittleEndian)
            {
                //         00 01 02 03
                // BUFFER: DD CC BB AA
                // VALUE:  AA BB CC DD

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

        public IPAddress ReadIPAddress()
        {
            var span = new Span<byte>(buffer, offset, 4);
            return new IPAddress(span);
        }

        public string ReadString(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (buffer[offset + i] == 0)
                {
                    count = i;
                    break;
                }
            }

            var value = Encoding.ASCII.GetString(this.buffer, this.offset, count);
            this.offset += count;

            return value;
        }

        public bool ReadBoolean()
        {
            byte value = this.buffer[offset++];

            return value == 1 ? true : false;
        }

        public string ReadString()
        {
            int count = 0;

            for (int i = 0; i < count; i++)
            {
                if (buffer[offset + i] == 0)
                {
                    count = i;
                    break;
                }
            }

            var value = Encoding.ASCII.GetString(this.buffer, this.offset, count);
            this.offset += count;

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
    }
}
