using FileUploadExtension;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MyBlog.BlogSystem.Domain.AppSettings;
using MyBlog.BlogSystem.Domain.Managers;
using MyBlog.BlogSystem.Domain.SensitiveInfo;
using MyBlog.BlogSystem.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using static MyBlog.BlogSystem.Domain.SensitiveInfo.SensitiveWordsLibrary;
using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using MyBlog.BlogSystem.Application.Contract.SensitiveApp;
using DistributedLock;

namespace MyBlog.BlogSystem.Application.SensitiveApp
{
    public class SensitiveService : ApplicationService,ISensitiveService
    {
        private readonly IDistributedCache _cache;
        private readonly MyBlogSystemDbContext _context;
        private readonly IOptions<UploadSettings> _uploadSettings;
        private readonly DistributedLockService _distributedLockService;
        private readonly SensitiveWordsLibraryManager _sensitiveWordsLibraryManager;
        private static readonly HttpClient _httpClient = new HttpClient(); // 复用 HttpClient，提高性能

        public SensitiveService(
            IOptions<UploadSettings> uploadSettings,
            IDistributedCache cache,
            MyBlogSystemDbContext context,
            DistributedLockService distributedLockService,
            SensitiveWordsLibraryManager sensitiveWordsLibraryManager)
        {
            _cache = cache;
            _context = context;
            _uploadSettings = uploadSettings;
            _distributedLockService = distributedLockService;
            _sensitiveWordsLibraryManager = sensitiveWordsLibraryManager;
        }

        public async Task SaveFile(IFormFile sensitiveFile)
        {
            var uploadContext = new UploadContext(sensitiveFile, Guid.NewGuid().ToString("N"), _uploadSettings.Value.sensitiveAddress, overwrite: true);
            await uploadContext.FileUploadAsync().FileUploadAsync(
                async FileInfo =>
                {
                    uploadContext.FilePath = FileInfo;
                    await _sensitiveWordsLibraryManager.AddSenitiveWordsLibrary(uploadContext);
                },
                FileInfo =>
                {
                    throw new InvalidOperationException("当前文件已经存在");
                },
                ex =>
                {
                    throw ex;
                });
        }

        /// <summary>
        /// 控制开启和关闭敏感词库
        /// </summary>
        public async Task SetLibraryStatusAsync(string LibraryId, SenstiveStatus status)
        {
            await _context.Set<SensitiveWordsLibrary>().Where(m => m.Id == LibraryId)
                  .ExecuteUpdateAsync(m => m.SetProperty(p => p.Statue, status));
        }

        public async Task SetWordsInCacheAsync()
        {
            List<string> lineList = new();
            
            WebClient webClient = new WebClient();
            // 获取启用的敏感词库
            var sensitiveLibraries = await _sensitiveWordsLibraryManager.GetSensitiveWorldsLibrariesInActiveAsync();
            foreach (var library in sensitiveLibraries)
            {
                try
                {
                    string text;

                    if (library.LibraryFileUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        // 处理远程 URL
                        text = await _httpClient.GetStringAsync(library.LibraryFileUrl);
                    }
                    else
                    {
                        // 处理本地文件
                        if (!File.Exists(library.LibraryFileUrl))
                        {
                            Console.WriteLine($"文件不存在: {library.LibraryFileUrl}");
                            continue;
                        }

                        text = await File.ReadAllTextAsync(library.LibraryFileUrl);
                    }

                    var words = text.Split(new[] { "\r\n", "，" }, StringSplitOptions.RemoveEmptyEntries);
                    lineList.AddRange(words);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"读取文件失败: {library.LibraryFileUrl}, 错误: {ex.Message}");
                }
            }

            // 去重处理
            lineList = lineList.Distinct().ToList();
            lineList.Remove("");

            // 设置缓存数据
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
            };
            await _cache.SetStringAsync("SensitiveWords", JsonSerializer.Serialize(lineList), cacheOptions);
        }

        /// <summary>
        /// 从缓存中获取敏感词列表
        /// </summary>
        public async Task<List<string>> GetWordsInCacheAsync()
        {
            var cacheKey = "SensitiveWords";
            var wordsJson = await _cache.GetStringAsync(cacheKey);

            if (wordsJson == null)
            {
                var wordsList = await LoadAndCacheSensitiveWordsAsync(cacheKey);
                return wordsList;
            }
            return JsonSerializer.Deserialize<List<string>>(wordsJson) ?? new List<string>();
        }

        /// <summary>
        /// 加载敏感词库并存入缓存
        /// </summary>
        private async Task<List<string>> LoadAndCacheSensitiveWordsAsync(string cacheKey)
        {
            // 尝试获取分布式锁
            if (!await _distributedLockService.TryAcquireLockAsync())
            {
                throw new InvalidOperationException("无法获取分布式锁，稍后再试");
            }

            try
            {
                // 尝试从缓存中获取敏感词列表
                var wordsJson = await _cache.GetStringAsync(cacheKey);
                if (wordsJson != null)
                {
                    // 如果缓存中有敏感词列表，直接返回
                    return JsonSerializer.Deserialize<List<string>>(wordsJson) ?? new List<string>();
                }

                // 如果缓存中没有敏感词，开始加载敏感词库
                List<string> lineList = new();
                var sensitiveLibraries = await _sensitiveWordsLibraryManager.GetSensitiveWorldsLibrariesInActiveAsync();

                foreach (var library in sensitiveLibraries)
                {
                    try
                    {
                        string text;

                        if (library.LibraryFileUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            // 处理远程 URL
                            text = await _httpClient.GetStringAsync(library.LibraryFileUrl);
                        }
                        else
                        {
                            // 处理本地文件
                            if (!File.Exists(library.LibraryFileUrl))
                            {
                                Console.WriteLine($"文件不存在: {library.LibraryFileUrl}");
                                continue;
                            }

                            text = await File.ReadAllTextAsync(library.LibraryFileUrl);
                        }

                        var words = text.Split(new[] { "\r\n", "，" }, StringSplitOptions.RemoveEmptyEntries);
                        lineList.AddRange(words);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"读取文件失败: {library.LibraryFileUrl}, 错误: {ex.Message}");
                    }
                }

                // 去重并存入缓存
                lineList = lineList.Distinct().ToList();
                lineList.Remove("");

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) // 设置缓存的过期时间
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(lineList), cacheOptions);
                return lineList;
            }
            finally
            {
                // 释放锁
                await _distributedLockService.ReleaseLockAsync("SensitiveWordsLock");
            }
        }
    }
}
