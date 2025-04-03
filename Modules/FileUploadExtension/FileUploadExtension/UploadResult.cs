using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadExtension
{
    public class UploadResult
    {
        public bool IsSuccess { get; set; }
        public string FileName { get; set; }
        public Exception Error { get; set; }
        public bool FileExists
        {
            get; set;
        }
    }
}
