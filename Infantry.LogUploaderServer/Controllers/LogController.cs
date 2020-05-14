using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Infantry.LogUploader.Common.Models;
using Infantry.LogUploader.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infantry.LogUploaderServer.Controllers
{
    [ApiController]
    [Route("Logs")]
    public class LogController : ControllerBase
    {
        private readonly LogUploaderConfiguration configuration;
        public LogController(IOptions<LogUploaderConfiguration> configuration)
        {
            this.configuration = configuration.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            this.CreateLogDirectoryIfNotExists();

            var files = Directory.GetFiles(this.configuration.InfantryLogDirectory);

            return this.Ok(files
                .Select(path => new FileInfo(path))
                .Select(info => new
                {
                    FileName = info.Name,
                    Size = info.Length
                }));
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var file = this.Request.Form.Files.FirstOrDefault();

            if (file == null)
            {
                return this.Error("No file was uploaded.");
            }

            if (this.GetTotalLogSize() > this.configuration.MaxDirectorySize)
            {
                return this.Error("Log file capacity has been reached. Please try again later.");
            }

            this.CreateLogDirectoryIfNotExists();

            string fileName = this.GetPackageFileName();

            using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }

            return this.Ok();
        }

        private void CreateLogDirectoryIfNotExists()
        {
            if (!Directory.Exists(this.configuration.InfantryLogDirectory))
            {
                Directory.CreateDirectory(this.configuration.InfantryLogDirectory);
            }
        }

        private string GetPackageFileName()
        {
            string fileName;        
            var now = DateTime.Now;

            int attempts = 0;
            do
            {
                string count = "";
                if (attempts > 0)
                {
                    count = $".{attempts}";
                }

                fileName = Path.Combine(this.configuration.InfantryLogDirectory,
                    $"{now:yyyy-MM-dd.HHmmss}{count}.inflog.zip");
                attempts++;
            } while (System.IO.File.Exists(fileName));

            return fileName;
        }

        private IActionResult Error(string message)
        {
            var response = ResponseBase.CreateErrorResponse(message);

            return this.BadRequest(response);
        }

        private long GetTotalLogSize()
        {
            var directory = new DirectoryInfo(this.configuration.InfantryLogDirectory);

            return directory.GetFiles().Sum(f => f.Length);
        }
    }
}
