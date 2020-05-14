namespace Infantry.LogUploader.Client
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Infantry.LogUploader.Common.Models;

    public class LogUploaderClient : ILogUploaderClient
    {
        // HttpClient is garbage, and should be static.
        private static readonly HttpClient client = new HttpClient();
        private readonly Uri url;

        public LogUploaderClient(Uri url)
        {
            this.url = url;
        }

        public async Task<ResponseBase> UploadPackageAsync(LogPackage package)
        {
            using var content = new MultipartFormDataContent();

            using var stream = new FileStream(package.FileName, FileMode.Open, FileAccess.Read);
            var streamContent = new StreamContent(stream);
            streamContent.Headers.Add("Content-Type", "application/octet-stream");

            content.Add(streamContent, "file", Path.GetFileName(package.FileName));

            var response = await client.PostAsync(this.GetUrl("logs"), content);

            return await this.DeserializeResponseAsync<ResponseBase>(response);
        }

        private Uri GetUrl(string resource)
        {
            var builder = new UriBuilder(this.url);

            builder.Path += resource + "/";

            return builder.Uri;
        }

        private async Task<TResponse> DeserializeResponseAsync<TResponse>(HttpResponseMessage message)
        {
            var responseText = await message.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseText)) {
                responseText = "{}";
            }

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Deserialize<TResponse>(responseText, options);
        }
    }
}
