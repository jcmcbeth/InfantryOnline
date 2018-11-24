namespace Infantry.Tests
{
    using Infantry.Directory;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net;
    using System.Threading.Tasks;

    [TestClass]
    public class DirectoryClientTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public async Task GetZones_ValidServer_ZonesReturned()
        {
            using (var target = new DirectoryClient(IPAddress.Parse("108.61.133.122")))
            {
                var actual = await target.GetZonesAsync();

                Assert.AreNotEqual(0, actual.Count);
            }
        }
    }
}
