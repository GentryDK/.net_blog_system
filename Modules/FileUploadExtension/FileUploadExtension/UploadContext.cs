using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadExtension
{
    public class UploadContext
    {
        public IFormFile File { get; set; }
        public string FileId { get; set; }
        public string ServerUrl { get; set; }
        public bool Overwrite { get; set; }
        public string FilePath { get; set; }

        public UploadContext(IFormFile file, string fileId, string serverUrl, bool overwrite = false)
        {
            File = file;
            FileId = fileId;
            ServerUrl = serverUrl;
            Overwrite = overwrite;
        }
    }
}
