using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly string logFilePath;
        public LogController(IOptions<LogUploaderConfiguration> configuration)
        {
            this.logFilePath = configuration.Value.InfantryLogPath;
        }

        [HttpGet]
        public IActionResult Get()
        {
            this.CreateLogDirectoryIfNotExists();

            var files = Directory.GetFiles(this.logFilePath);

            return this.Ok(files);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var file = this.Request.Form.Files.FirstOrDefault();

            if (file == null)
            {
                return this.Error("No file was uploaded.");
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
            if (!Directory.Exists(this.logFilePath))
            {
                Directory.CreateDirectory(this.logFilePath);
            }
        }

        private string GetPackageFileName()
        {
            string fileName;        
            var now = DateTime.Now;

            int attempts = 0;
            do
            {
                string prefix = "";
                if (attempts > 0)
                {
                    prefix = $"{attempts}.";
                }

                fileName = Path.Combine(this.logFilePath, prefix + $"{now:yyyy-MM-dd.HHmmss}.inflog.zip");
                attempts++;
            } while (System.IO.File.Exists(fileName));

            return fileName;
        }

        private IActionResult Error(string message)
        {
            var response = ResponseBase.CreateErrorResponse(message);

            return this.BadRequest(response);
        }
    }
}
