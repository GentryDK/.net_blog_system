using System.Net.Http.Headers;

namespace MyBlog.UserSystem.web.Extensions
{
    public static class UploadExtenstion
    {
       public static async Task<string> UploadAsync<T>(this IFormFile file,string fileId,string serverUrl, Func<string,Task<T>> uploadingTheDatabase) 
        {
            if (file == null || file.Length == 0)
            {
                return "no file uploaded.";
            }
            // 生成唯一文件名
            var fileExtension = Path.GetExtension(file.FileName);
            string fileName = $"{fileId}{fileExtension}";

            try
            {
                //确保目录存在
                Directory.CreateDirectory(serverUrl);
                //保存文件到指定目录
                string savePath = Path.Combine(serverUrl, fileName);
                using (var FileStream = new FileStream(savePath, FileMode.Create))
                {
                    await file.CopyToAsync(FileStream);
                }
                await uploadingTheDatabase(fileName);
                return fileName;

            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
