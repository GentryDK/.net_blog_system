using MyBlog.BlogSystem.Domain.PostInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace MyBlog.BlogSystem.Domain.Managers
{
    public class PostTypeManager : DomainService
    {
        public readonly IRepository<PostType> PostTypeRepo;

        public PostTypeManager(IRepository<PostType> postTypeRepo)
        {
            this.PostTypeRepo = postTypeRepo;
        }

        /// <summary>
        /// 拿到全部的PostType(根据pageIndex和pageSize)
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<PostType>> GetPostTypeAsync(int pageIndex, int pageSize = 5)
        {
            var query = await PostTypeRepo.GetQueryableAsync();
            var postTypes = query.Where(m=>m.IsDeleted=="F").Skip(pageIndex).Take(pageSize).ToList();
            return postTypes;
        }

        /// <summary>
        /// 根据PostTypeName拿到对应的PostType
        /// </summary>
        /// <param name="postTypeName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<PostType> GetPostTypeByTypeNameAsync(string postTypeName)
        {
            if (postTypeName == null && postTypeName == "")
            {
                throw new ArgumentNullException(nameof(postTypeName));
            }
            return await PostTypeRepo.FirstOrDefaultAsync(m => m.PostTypeName == postTypeName && m.IsDeleted== "F");
        }

        public async Task<PostType> GetPostTypeById(string postTypeId)
        {
            return await PostTypeRepo.FirstOrDefaultAsync(m=>m.Id== postTypeId && m.IsDeleted == "F");
        }

        /// <summary>
        /// 得到当前的PostType的数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetPostTypeCountAsync()
        {
            return await PostTypeRepo.CountAsync(m=>m.IsDeleted=="F");
        }
    }
}
