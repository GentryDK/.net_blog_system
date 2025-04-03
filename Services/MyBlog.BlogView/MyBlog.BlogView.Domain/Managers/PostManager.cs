using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogSystem.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.ObjectMapping;
using static System.Collections.Specialized.BitVector32;
using Volo.Abp.Uow;
using System.Drawing;
using MyBlog.BlogSystem.Domain.Shared.Ctos;
using MyBlog.BlogSystem.Domain.Extensions;

namespace MyBlog.BlogSystem.Domain.Managers
{
    public class PostManager : DomainService
    {
        private readonly IRepository<Post> PostRepo;
        private readonly IRepository<PostType> PostTypeRepo;

        public IQueryable<Post> PostQueryable => PostRepo.GetQueryableAsync().Result;

        public PostManager(
            IRepository<Post> postRepo,
            IRepository<PostType> postTypeRepo
            )
        {
           this.PostRepo= postRepo;
           this.PostTypeRepo= postTypeRepo;
        }

        /// <summary>
        /// 根据帖子类型获取到帖子
        /// </summary>
        /// <param name="filters">帖子的过滤条件</param>
        /// <param name="skip">拿的页数</param>
        /// <param name="userId">需要拿的用户,null则拿到全部用户的</param>
        /// <returns></returns>
        public async Task<(List<Post> posts, int totalCount)> GetFilteredPostsWithCountAsync(PostFilter filters, int skip)
        {
            var query = await PostRepo.GetQueryableAsync();

            // 应用过滤条件
            query = query.FilterByPostTitle(filters.PostTitle)
                         .FilterByPostTypeId(filters.PostTypeId)
                         .ExcludeOpenPosts();

            // 获取符合条件的总数
            var totalCount = await query.CountAsync();

            // 获取分页后的帖子
            var posts = await query.OrderByDescending(post => post.CreateDate)
                           .Skip(skip)
                           .Take(filters.pageSize)
                           .ToListAsync();

            return (posts, totalCount);
        }

        /// <summary>
        /// 通过帖子类型获取到帖子
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public async Task<List<Post>> GetPostsByTypeIdAsync(string typeId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(typeId);
            var query = await PostRepo.GetQueryableAsync();
            return await query.Where(m => m.PostTypeId == typeId).ToListAsync();
        }

        /// <summary>
        /// 通过专题拿到帖子
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="size"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public async Task<List<Post>> GetPostsBySubjectAsync(int skip, int size, string subjectId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(subjectId);

            var query = await PostRepo.GetQueryableAsync();
            var posts = query.Where(m => m.SubjectId == subjectId).Skip(skip).Take(size).ToList();
            return await GetPostsInfoAsync(posts);
        }

        public async Task<int> GetPostCountByTypeAsymc(string typeId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(typeId);

            var count = await PostRepo.CountAsync(m => m.PostTypeId == typeId);
            return count;
        }

        /// <summary>
        /// 通过id拿到对应的Post
        /// </summary>
        /// <returns></returns>
        public async Task<Post> GetPostByIdAsnyc(string Id)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(Id);

            return await PostRepo.FirstOrDefaultAsync(m => m.Id == Id && m.IsClose == "F");
        }

        public async Task<int> GetPostByUserIdAsnyc(string Id)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(Id);
            var query = await PostRepo.GetQueryableAsync();
            return await query.CountAsync(p=>p.CreateUserId==Id && p.IsClose=="F");
        }

        /// <summary>
        /// 获取帖子详细信息
        /// </summary>
        /// <param name="postsArray"></param>
        /// <returns></returns>
        public async Task<List<Post>> GetPostsInfoAsync(List<Post> posts)
        {
            ArgumentNullException.ThrowIfNull(posts);

            var postTypeIds = posts.Select(m => m.PostTypeId);
            var postTypes = await PostTypeRepo.GetListAsync(m=>postTypeIds.Contains(m.Id));

            posts.ForEach(m =>
            {
                m.PostType = postTypes.FirstOrDefault(n=>n.Id == m.PostTypeId);
            });

            return posts;
        }
    }
}
