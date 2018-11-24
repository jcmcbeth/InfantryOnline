﻿namespace Infantry.Directory
{
    using Infantry.Network;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class DirectoryClient : IDirectoryClient, IDisposable
    {
        public const int DefaultPort = 4850;

        private readonly UdpClient client;
        private bool isDisposed = false;

        public DirectoryClient(IPAddress address, int port = DefaultPort)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            var endpoint = new IPEndPoint(address, port);

            this.client = new UdpClient();
            this.client.Connect(endpoint);
        }

        public async Task<ICollection<Zone>> GetZonesAsync()
        { 
            await this.HandleChallengeRequest();
            await this.HandleClientRequest();

            return await this.HandleZoneListRequest();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing && this.client != null)
                {
                    this.client.Dispose();
                }

                isDisposed = true;
            }
        }

        private async Task<ICollection<Zone>> HandleZoneListRequest()
        {
            var zones = new List<Zone>();
            var builder = new PacketBuilder();
            

            builder.AddInt16(0x0005);
            builder.AddBytes(new byte[26]);

            var buffer = builder.GetBytes();

            await this.client.SendAsync(buffer, buffer.Length);
            
            int chunkBufferLength;
            int chunk;
            int offset = 0;
            byte[] chunkBuffer = null;

            do
            {
                buffer = (await this.client.ReceiveAsync()).Buffer;
                PacketReader reader = new PacketReader(buffer);

                reader.ReadInt16();
                reader.ReadInt16();
                chunk = reader.ReadByte();
                reader.ReadBytes(8);
                chunkBufferLength = reader.ReadInt16();
                reader.ReadInt16();

                if (chunkBuffer == null)
                {
                    chunkBuffer = new byte[chunkBufferLength];
                }

                var zone = new Zone();

                zone.ServerAddress = reader.ReadIPAddress();
                zone.ServerPort = reader.ReadInt16();
                reader.ReadBytes(6);
                zone.Name = reader.ReadString(32);
                reader.ReadInt16();
                zone.IsAdvanced = reader.ReadBoolean();
                reader.ReadBytes(29);
                //zone.Description 

                zones.Add(zone);

                builder = new PacketBuilder();
                builder.AddInt16(0x000b);
                builder.AddBytes(new byte[3]);
                builder.AddByte((byte)chunk);
                builder.AddInt16(0);

                buffer = builder.GetBytes();

                await this.client.SendAsync(buffer, buffer.Length);

                chunk++;

                offset += buffer.Length - 17;
            } while (offset <= chunkBufferLength);

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

            await this.client.SendAsync(buffer, buffer.Length);

            var result = await this.client.ReceiveAsync();

            PacketReader reader = new PacketReader(result.Buffer);

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
            builder.AddInt16(0x0002);       
            builder.AddBytes(new byte[6]);
            builder.AddByte(0x01);
            builder.AddString("Infantry", 9);
            builder.AddBytes(new byte[16]);

            var buffer = builder.GetBytes();

            await this.client.SendAsync(buffer, buffer.Length);
        }
    }
}