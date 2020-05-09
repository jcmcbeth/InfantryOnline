using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infantry.LogScanner
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var scanner = new FileScanner();

            var files = scanner.GetFiles(@"^Message\d{4}-\d{2}-\d{2}\.log$");

            foreach (var file in files)
            {
                Console.WriteLine(file);
            }

            string fileName = null;
            try
            {
                var packager = new LogPackager();
                fileName = packager.CreatePackage(files);

                var uploader = new LogUploader(new Uri("https://localhost:44316"));
                await uploader.UploadLogPackage(fileName);
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }

            Console.WriteLine($"File: {fileName}");
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }
    }
}
