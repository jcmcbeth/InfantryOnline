namespace Infantry.Tests
{
    using Infantry.Client.Directory;
    using Infantry.Network;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    [TestClass]
    public class DirectoryClientTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public async Task GetZones_ValidServer_ZonesReturned()
        {
            using (var udpClient = new UdpClient())
            {
                var networkClient = new UdpNetworkClient(udpClient);
                var target = new DirectoryClient(networkClient, IPAddress.Parse("108.61.133.122"));

                var actual = await target.GetZonesAsync();

                Assert.AreNotEqual(0, actual.Count);
            }
        }
    }
}
