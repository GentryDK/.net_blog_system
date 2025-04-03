using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using static MyBlog.BlogSystem.Domain.SensitiveInfo.SensitiveWordsLibrary;

namespace MyBlog.BlogView.Application.Contract.SensitiveApp
{
    public interface ISensitiveService: IApplicationService
    {
        Task<List<string>> GetWordsInCacheAsync();
    }
}
