using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Infantry.LogUploader.Client;

namespace Infantry.LogUploader.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var arguments = ArgumentParser.ParseArguments(args);

                var scanner = new FileScanner();

                var files = scanner.GetFiles(@"^Message\d{4}-\d{2}-\d{2}\.log$");

                LogPackage package = null;
                try
                {
                    var packager = new LogPackager();
                    package = packager.CreatePackage(files);

                    var uploader = new LogUploaderClient(arguments.ServiceUrl);
                    var result = await uploader.UploadPackageAsync(package);

                    if (result.Errors?.Any() == true)
                    {
                        Console.Error.WriteLine("Failed to upload file:");
                        foreach (var error in result.Errors)
                        {
                            Console.Error.WriteLine(error);
                        }
                    }
                }
                finally
                {
                    if (package != null)
                    {
                        package.Delete();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"Fatal error: {exception.Message}");
            }
        }
    }
}
