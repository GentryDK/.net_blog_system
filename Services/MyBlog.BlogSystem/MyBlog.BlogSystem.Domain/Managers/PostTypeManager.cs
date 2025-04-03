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
        /// 插入新的PostType
        /// </summary>
        /// <param name="postType"></param>
        /// <returns></returns>
        public async Task<PostType> SetPostTypeAsync(PostType postType)
        {
            var count = await PostTypeRepo.GetCountAsync();
            postType.InitPostType(Convert.ToInt32(count));
            var isSuccess = await PostTypeRepo.InsertAsync(postType);
            return isSuccess;
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
        /// 更新当前已有的PostType
        /// </summary>
        /// <param name="postType"></param>
        /// <returns></returns>
        public async Task<PostType> UpdatePostTypeAsync(PostType postType)
        {
            return await PostTypeRepo.UpdateAsync(postType);
        }

        /// <summary>
        /// 更新当前帖子封面
        /// </summary>
        /// <param name="fileCover"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePostTypeCoverUrl(string fileCover)
        {
            var cover = Path.GetFileName(fileCover);
            var PostTypeId = cover.Split('.');
            var PostType = await PostTypeRepo.GetAsync(m=>m.Id == PostTypeId[0]);
            if (PostType!=null)
            {
                PostType.Cover = fileCover;
                await PostTypeRepo.UpdateAsync(PostType);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 得到当前的PostType的数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetPostTypeCountAsync()
        {
            return await PostTypeRepo.CountAsync();
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> SoftPostTypeAsync(string postTypeId)
        {
            var postType = await PostTypeRepo.GetAsync(m => m.Id == postTypeId);
            postType.IsDeleted = "T";
            await PostTypeRepo.UpdateAsync(postType);
            return true;
        }

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> HardDeletePostTypeAsync(string id)
        {
            await PostTypeRepo.DeleteAsync(m =>m.Id==id);
            return true;
        }

        /// <summary>
        /// 硬删除全部
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> HardDeletePostTypesAsync()
        {
            await PostTypeRepo.DeleteAsync(m=>m.IsDeleted=="T");
            return true;
        }
    }
}
