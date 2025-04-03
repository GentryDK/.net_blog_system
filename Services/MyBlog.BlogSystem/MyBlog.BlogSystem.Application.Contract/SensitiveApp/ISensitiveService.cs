using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using static MyBlog.BlogSystem.Domain.SensitiveInfo.SensitiveWordsLibrary;

namespace MyBlog.BlogSystem.Application.Contract.SensitiveApp
{
    public interface ISensitiveService: IApplicationService
    {
        Task SaveFile(IFormFile sensitiveFile);

        Task SetLibraryStatusAsync(string LibraryId, SenstiveStatus status);

        Task SetWordsInCacheAsync();

        Task<List<string>> GetWordsInCacheAsync();
    }
}
