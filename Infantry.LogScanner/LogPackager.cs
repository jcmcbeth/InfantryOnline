namespace Infantry.LogUploader.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    public class LogPackager
    {
        public LogPackage CreatePackage(IEnumerable<string> files)
        {
            var fileName = Path.GetTempFileName();

            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                using var archive = new ZipArchive(stream, ZipArchiveMode.Create);
                int id = 1;
                foreach (var fullPath in files)
                {
                    var entryName = $"{id++}.{Path.GetFileName(fullPath)}";
                    archive.CreateEntryFromFile(fullPath, entryName, CompressionLevel.Optimal);
                }
            }

            var package = new LogPackage()
            {
                FileName = fileName,
            };
            package.SourcePaths.AddRange(files);

            return package;
        }
    }
}
