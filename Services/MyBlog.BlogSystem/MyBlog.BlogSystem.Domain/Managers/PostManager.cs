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
        private readonly IRepository<Reply> ReplyRepo;

        public IQueryable<Post> PostQueryable => PostRepo.GetQueryableAsync().Result;

        public PostManager(
            IRepository<Reply> replyRepo,
            IRepository<Post> postRepo,
            IRepository<PostType> postTypeRepo
            )
        {
           this.PostRepo= postRepo;
           this.ReplyRepo= replyRepo;
           this.PostTypeRepo= postTypeRepo;
        }

        /// <summary>
        /// 添加帖子
        /// </summary>
        /// <returns></returns>
        public async Task<Post> AddPostAsync(Post post,string userId,string userName)
        {
            post.InitPost(userId,userName);
            post = await PostRepo.InsertAsync(post);
            return post;
        }

        /// <summary>
        /// 更新当前已有的Post
        /// </summary>
        /// <param name="postType"></param>
        /// <returns></returns>
        public async Task<Post> UpdatePostAsync(Post post)
        {
            return await PostRepo.UpdateAsync(post);
        }

        /// <summary>
        /// 根据帖子类型获取到帖子
        /// </summary>
        /// <param name="filters">帖子的过滤条件</param>
        /// <param name="skip">拿的页数</param>
        /// <param name="userId">需要拿的用户,null则拿到全部用户的</param>
        /// <returns></returns>
        public async Task<(List<Post> posts, int totalCount)> GetFilteredPostsWithCountAsync(PostFilter filters, int skip, string? userId = null)
        {
            var query = await PostRepo.GetQueryableAsync();

            // 应用过滤条件
            query = query.FilterByUserId(userId)
                         .FilterByPostTitle(filters.PostTitle)
                         .FilterByState(filters.State)
                         .FilterByPostTypeId(filters.PostTypeId)
                         .ExcludeOpenPosts();

            // 获取符合条件的总数
            var totalCount = await query.CountAsync();

            // 获取分页后的帖子
            var posts = await query.OrderByDescending(post=>post.CreateDate).Skip(skip).Take(filters.pageSize).ToListAsync();

            return (posts, totalCount);
        }

        /// <summary>
        /// 获取到所有的被删除的帖子数据
        /// </summary>
        /// <returns></returns>
        public async Task<(List<Post> posts, int totalCount)> GetRecyclePostsAsync(int skip,int pageSize,string? postTitle,string? userId = null)
        {
            var query = await PostRepo.GetQueryableAsync();

            query = query.FilterByUserId(userId)
                         .FilterByPostTitle(postTitle)
                         .IncludeClosedPosts();

            var totalCount = await query.CountAsync();

            var posts = await query.Skip(skip).Take(pageSize).ToListAsync();

            return (posts,totalCount);
        }

        /// <summary>
        /// 通过帖子类型获取到帖子
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public async Task<List<Post>> GetPostByTypeIdAsync(string typeId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(typeId);
            var query = await PostRepo.GetQueryableAsync();
            return await query.Where(m => m.PostTypeId == typeId).ToListAsync();
        }

        public async Task<int> GetPostCountByTypeAsymc(string typeId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(typeId);

           var count = await PostRepo.CountAsync(m=>m.PostTypeId==typeId);
            return count;
        }

        /// <summary>
        /// 更新当前帖子封面
        /// </summary>
        /// <param name="fileCover"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePostCoverUrl(string fileCover)
        {
            var cover = Path.GetFileName(fileCover);
            var PostId = cover.Split('.');
            var Post = await PostRepo.GetAsync(m => m.Id == PostId[0]);
            if (Post != null)
            {
                Post.PostCover = fileCover;
                await PostRepo.UpdateAsync(Post);
                return true;
            }
            return false;
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

        /// <summary>
        /// 通过id拿到对应的Post
        /// </summary>
        /// <returns></returns>
        public async Task<Post> GetPostByIdAsnyc(string Id)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(Id);

            return await PostRepo.FirstOrDefaultAsync(m => m.Id == Id && m.IsClose == "F");
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

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> SoftPostAsync(string postId)
        {
            var post = await PostRepo.GetAsync(m => m.Id == postId);
            post.IsClose = "T";
            await PostRepo.UpdateAsync(post);
            return true;
        }

        /// <summary>
        /// 软删除当前PostType下面的所有的Post
        /// </summary>
        /// <param name="postTypeId"></param>
        /// <returns></returns>
        public async Task<bool> SoftPostTypeByTypeIdAsync(string postTypeId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(postTypeId);
           var posts = await GetPostByTypeIdAsync(postTypeId);
            if (posts == null) return false;

            posts.ForEach(p=>p.IsClose="T");
            await PostRepo.UpdateManyAsync(posts);
            return true;
        }

        /// <summary>
        /// 恢复被软删除的帖子
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Post> RecoverSoftPostAsync(string postId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(postId);
            var post = await PostRepo.GetAsync(m=>m.Id==postId);
            post.IsClose = "F";
            return await PostRepo.UpdateAsync(post);
        }

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> HardDeletePostAsync(string id)
        {
            await PostRepo.DeleteAsync(m => m.Id == id);
            var replies = await ReplyRepo.GetListAsync(r => r.PostId == id);
            foreach (var reply in replies)
            {
                await ReplyRepo.DeleteAsync(reply);
            }
            return true;
        }

        /// <summary>
        /// 硬删除全部
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> HardDeletePostAsync()
        {
            await PostRepo.DeleteAsync(m => m.IsClose == "T");
            return true;
        }

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> HardDeletePostByTpyeIdAsync(string postTypeId)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(postTypeId);
            var posts = await GetPostByTypeIdAsync(postTypeId);
            if (posts == null || posts.Count == 0) return false;

            var deletePost = await Task.Run(async () =>
            {
                return posts.Where(p => p.IsClose == "T").ToList();
            });
            if (deletePost.Count == 0) return false;
            await PostRepo.DeleteManyAsync(deletePost);
            return true;
        }
    }
}
