namespace Infantry.LogUploader.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class LogPackage
    {
        public string FileName { get; set; }

        public List<string> SourcePaths { get; } = new List<string>();

        public Stream GetStream()
        {
            return new FileStream(this.FileName, FileMode.Open, FileAccess.Read);
        }

        public void Delete()
        {
            if (!string.IsNullOrWhiteSpace(this.FileName) && File.Exists(this.FileName))
            {
                File.Delete(this.FileName);
            }
        }
    }
}
