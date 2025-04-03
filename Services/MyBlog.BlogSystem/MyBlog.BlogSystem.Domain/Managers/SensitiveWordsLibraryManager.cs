using FileUploadExtension;
using MyBlog.BlogSystem.Domain.SensitiveInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using static MyBlog.BlogSystem.Domain.SensitiveInfo.SensitiveWordsLibrary;

namespace MyBlog.BlogSystem.Domain.Managers
{
    public class SensitiveWordsLibraryManager: DomainService
    {
        private readonly IRepository<SensitiveWordsLibrary> _sensitiveWordsLibraryRepo;

        public SensitiveWordsLibraryManager(IRepository<SensitiveWordsLibrary> sensitiveWordsLibraryRepo) 
        {
            this._sensitiveWordsLibraryRepo = sensitiveWordsLibraryRepo;
        }

        public async Task AddSenitiveWordsLibrary(UploadContext uploadContext)
        {
            try
            {
                var model = new SensitiveWordsLibrary(uploadContext.FileId);
                model.InitData(uploadContext.FilePath, uploadContext.FileId);
                await _sensitiveWordsLibraryRepo.InsertAsync(model);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task UpdateStatusAsync(string libraryId, SenstiveStatus status)
        {
            var sensitiveWord = await _sensitiveWordsLibraryRepo.GetAsync(m => m.Id == libraryId);
            sensitiveWord.Statue = status;
            await _sensitiveWordsLibraryRepo.UpdateAsync(sensitiveWord);
        }

        /// <summary>
        /// 获得所有当前为激活状态的敏感词库
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<List<SensitiveWordsLibrary>> GetSensitiveWorldsLibrariesInActiveAsync()
        {
            return await _sensitiveWordsLibraryRepo.GetListAsync(m => m.Statue == SenstiveStatus.Enable);
        }
    }
}
