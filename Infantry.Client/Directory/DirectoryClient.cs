namespace Infantry.Client.Directory
{
    using Infantry.Directory;
    using Infantry.Network;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    public class DirectoryClient : IDirectoryClient
    {
        public const int DefaultPort = 4850;

        private readonly INetworkClient networkClient;

        public DirectoryClient(INetworkClient networkClient, IPAddress address, int port = DefaultPort)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            var endPoint = new IPEndPoint(address, port);

            this.networkClient = networkClient ?? throw new ArgumentNullException(nameof(networkClient));
            this.networkClient.Connect(endPoint);
        }

        public async Task<ICollection<Zone>> GetZonesAsync()
        { 
            await this.HandleChallengeRequest();
            await this.HandleClientRequest();

            return await this.HandleZoneListRequest();
        }

        private async Task<ICollection<Zone>> HandleZoneListRequest()
        {
            var zones = new List<Zone>();
            var builder = new PacketBuilder();   

            builder.AddInt16(0x0005);
            builder.AddBytes(new byte[26]);

            var buffer = builder.GetBytes();

            await this.networkClient.SendAsync(buffer, buffer.Length);
            
            int chunkBufferLength;
            int chunk;
            int offset = 0;
            byte[] chunkBuffer = null;
            PacketReader reader;

            do
            {
                buffer = await this.networkClient.ReceiveAsync();
                reader = new PacketReader(buffer);

                reader.ReadInt16();
                reader.ReadInt16();
                chunk = reader.ReadByte();
                reader.ReadBytes(7);
                chunkBufferLength = reader.ReadLittleEndianInt16();
                reader.ReadInt16();

                if (chunkBuffer == null)
                {
                    chunkBuffer = new byte[chunkBufferLength];
                }

                offset += reader.CopyBytes(chunkBuffer, offset);

                builder = new PacketBuilder();
                builder.AddInt16(0x000b);
                builder.AddInt16(0);
                builder.AddByte((byte)chunk);
                builder.AddPadding(3);

                buffer = builder.GetBytes();

                await this.networkClient.SendAsync(buffer, buffer.Length);

                chunk++;
            } while (offset < (chunkBufferLength));

            reader = new PacketReader(chunkBuffer);

            // Discard the 0x01 at the beginning
            reader.ReadByte();

            while (reader.CanRead())
            {
                var zone = new Zone();

                zone.ServerAddress = reader.ReadIPAddress();
                zone.ServerPort = reader.ReadUInt16();
                reader.ReadBytes(6);
                zone.Name = reader.ReadString(32);
                reader.ReadInt16();
                zone.IsAdvanced = reader.ReadBoolean();
                reader.ReadBytes(29);
                zone.Description = reader.ReadString();

                zones.Add(zone);
            }

            return zones;
        }

        private async Task HandleChallengeRequest()
        {
            var random = new Random();

            var token = random.Next();

            var builder = new PacketBuilder();
            builder.AddInt16(0x0001);
            builder.AddByte(0x03);
            builder.AddByte(0x00);
            builder.AddInt32(token);

            var buffer = builder.GetBytes();

            await this.networkClient.SendAsync(buffer, buffer.Length);

            var result = await this.networkClient.ReceiveAsync();

            PacketReader reader = new PacketReader(result);

            reader.ReadInt16();
            reader.ReadByte();
            reader.ReadByte();

            var responseToken = reader.ReadInt32();

            if (responseToken != token)
            {
                throw new InvalidOperationException("Invalid token response from server.");
            }         
        }

        private async Task HandleClientRequest()
        {
            var builder = new PacketBuilder();
            builder.AddInt16(0x0003);       
            builder.AddBytes(new byte[6]);
            builder.AddByte(0x01);
            builder.AddString("Infantry", 9);
            builder.AddBytes(new byte[16]);

            var buffer = builder.GetBytes();

            await this.networkClient.SendAsync(buffer, buffer.Length);
        }
    }
}
