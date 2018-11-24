namespace Infantry.Directory
{
    using System.Net;

    public class Zone
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsAdvanced { get; set; }

        public IPAddress ServerAddress { get; set; }

        public int ServerPort { get; set; }
    }
}