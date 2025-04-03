using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto;
using Volo.Abp.Application.Dtos;

namespace MyBlog.BlogSystem.Application.Contract.PostApp.Dto
{
    public class PostListDisplayDto : EntityDto<string>
    {
        /// <summary>
        /// 帖子标题
        /// </summary>
        public string PostTitle { get; set; }

        /// <summary>
        /// 帖子的标题图片
        /// </summary>
        public string PostCover { get; set; }


        /// <summary>
        /// 该帖子属于的板块
        /// </summary>
        public string PostTypeId { get; set; }

        /// <summary>
        /// 该帖子创建的用户ID
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 该帖子创建的用户名
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 帖子的概要
        /// </summary>
        public string? summary { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public long BrowseTime { get; set; }

        /// <summary>
        /// 帖子是否被删除
        /// </summary>
        public string IsClose { get; set; }

        public PostTypeDto PostTypeDto { get; set; }
    }
}
