namespace Infantry.Network
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class UdpNetworkClient : INetworkClient
    {
        private readonly UdpClient client;

        public UdpNetworkClient(UdpClient client)
        {
            this.client = client;
        }

        public void Connect(EndPoint endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            if (endpoint is IPEndPoint ipEndPoint)
            {
                this.client.Connect(ipEndPoint);
            }
            else
            {
                throw new ArgumentException("The endpoint must be an IPEndPoint.", nameof(endpoint));
            }
        }

        public async Task<byte[]> ReceiveAsync()
        {
            return (await this.client.ReceiveAsync()).Buffer;
        }

        public Task SendAsync(byte[] buffer, int length)
        {
            return this.client.SendAsync(buffer, length);
        }
    }
}
