using MyBlog.BlogSystem.Domain.Shared.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Application.Extension
{
    public static class BlogExtension
    {
        public static PostDraft EnsureInitialized(PostDraft draft)
        {
            return new PostDraft
            {
                Id = draft.Id ?? string.Empty,
                PostTitle = draft.PostTitle ?? string.Empty,
                summary = draft.summary ?? string.Empty,
                PostContent = draft.PostContent ?? string.Empty,
                PostTypeId = draft.PostTypeId ?? string.Empty,
                PostTypeName = draft.PostTypeName ?? string.Empty,
                Discuss = draft.Discuss ?? string.Empty,
                State = 1
            };
        }
    }
}
