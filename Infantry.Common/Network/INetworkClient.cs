namespace Infantry.Network
{
    using System.Net;
    using System.Threading.Tasks;

    public interface INetworkClient
    {
        void Connect(EndPoint endPoint);
        Task<byte[]> ReceiveAsync();
        Task SendAsync(byte[] buffer, int length);
    }
}
