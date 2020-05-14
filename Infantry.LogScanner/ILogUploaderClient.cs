namespace Infantry.LogUploader.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Infantry.LogUploader.Common.Models;

    public interface ILogUploaderClient
    {
        Task<ResponseBase> UploadPackageAsync(LogPackage package);
    }
}
