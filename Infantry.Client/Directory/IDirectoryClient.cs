namespace Infantry.Client.Directory
{
    using Infantry.Directory;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IDirectoryClient
    {
        Task<ICollection<Zone>> GetZonesAsync();
    }
}
