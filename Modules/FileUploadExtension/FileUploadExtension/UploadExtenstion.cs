using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadExtension
{
    public static class UploadExtenstion
    {
        public static async Task<UploadResult> FileUploadAsync(this UploadContext context)
        {
            if (context.File == null || context.File.Length == 0)
            {
                return new UploadResult { IsSuccess = false, Error = new Exception("no file uploaded.") };
            }

            var fileExtension = Path.GetExtension(context.File.FileName);
            string fileName = $"{context.FileId}{fileExtension}";

            try
            {
                Directory.CreateDirectory(context.ServerUrl);
                string savePath = Path.Combine(context.ServerUrl, fileName);

                if (!context.Overwrite && File.Exists(savePath))
                {
                    return new UploadResult 
                    { 
                        IsSuccess = false, 
                        FileExists = true, 
                        FileName = savePath
                    };
                }

                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    await context.File.CopyToAsync(fileStream);
                }

                return new UploadResult { IsSuccess = true, FileName = savePath };
            }
            catch (Exception ex)
            {
                return new UploadResult { IsSuccess = false, Error = ex };
            }
        }

        public static async Task<string> FileUploadAsync(this Task<UploadResult> uploadTask, Func<string, Task> onSuccess, Action<string> onExists, Action<Exception> onError)
        {
            var result = await uploadTask;
            if (result.IsSuccess)
            {
                await onSuccess(result.FileName);
            }
            else if (result.FileExists)
            {
                onExists(result.FileName);
            }
            else
            {
                onError(result.Error);
            }
            return result.FileName;
        }
    }
}
