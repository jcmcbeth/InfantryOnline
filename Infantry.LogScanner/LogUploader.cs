namespace Infantry.LogScanner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class LogUploader
    {
        private readonly Uri url;

        public LogUploader(Uri url)
        {
            this.url = url;
        }

        public async Task UploadLogPackage(string fileName)
        {
            using var client = new HttpClient();
            var uploadUrl = this.GetResourceUrl("Logs");

            using var content = new MultipartFormDataContent();

            using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var streamContent = new StreamContent(stream);

            content.Add(streamContent);

            var response = await client.PostAsync(uploadUrl, content);

            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        private Uri GetResourceUrl(string resource)
        {
            var builder = new UriBuilder(this.url);

            builder.Path += resource + "/";

            return builder.Uri;
        }
    }
}
