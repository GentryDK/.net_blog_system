using Microsoft.EntityFrameworkCore;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogSystem.Domain.Shared.Ctos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Domain.Extensions
{
    public static class PostQueryExtensions
    {
        public static IQueryable<Post> FilterByUserId(this IQueryable<Post> query, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return query;

            return query.Where(m => m.CreateUserId == userId);
        }

        public static IQueryable<Post> FilterByPostTitle(this IQueryable<Post> query, string? postTitle)
        {
            if (string.IsNullOrEmpty(postTitle))
                return query;

            return query.Where(m => EF.Functions.Like(m.PostTitle, $"%{postTitle}%"));
        }

        public static IQueryable<Post> FilterByState(this IQueryable<Post> query, int? state)
        {
            if (!state.HasValue)
                return query;

            return query.Where(m => m.State == state);
        }

        public static IQueryable<Post> FilterByPostTypeId(this IQueryable<Post> query, string? postTypeId)
        {
            if (string.IsNullOrEmpty(postTypeId))
                return query;

            return query.Where(m => m.PostTypeId == postTypeId);
        }

        public static IQueryable<Post> ExcludeOpenPosts(this IQueryable<Post> query)
        {
            return query.Where(m => m.IsClose == "F");
        }

        public static IQueryable<Post> IncludeClosedPosts(this IQueryable<Post> query)
        {
            return query.Where(m => m.IsClose == "T");
        }
    }
}
