using MyBlog.BlogSystem.Domain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace MyBlog.BlogSystem.Domain.PostInfo
{
    /// <summary>
    /// 发帖
    /// </summary>
    public class Post : AggregateRoot<string>
    {
        public Post()
        {

        }

        public Post(string id) : base(id)
        {

        }

        /// <summary>
        /// 帖子标题
        /// </summary>
        public string PostTitle { get; set; }

        /// <summary>
        /// 帖子的概要
        /// </summary>
        public string? summary {  get; set; }

        /// <summary>
        /// 帖子的标题图片
        /// </summary>
        public string? PostCover { get; set; }

        /// <summary>
        /// 帖子所属于的专题
        /// </summary>
        public string? SubjectId { get; set; }

        /// <summary>
        /// 该帖子属于的板块
        /// </summary>
        public string PostTypeId { get; set; }

        /// <summary>
        /// 帖子内容
        /// </summary>
        public string PostContent {  get; set; }

        /// <summary>
        /// 该帖子创建的用户ID
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 该帖子创建的用户名
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 是否可以评论
        /// </summary>
        public string Discuss { get; set; }

        /// <summary>
        /// 帖子的状态 0:发布 1:草稿
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime EditDate { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public long BrowseTime { get; set; }

        /// <summary>
        /// 帖子是否被删除
        /// </summary>
        public string IsClose { get; set; }

        public PostType PostType { get; set; }

        public void InitPost(string createUserId, string createUserName)
        {
            this.CreateUserId = createUserId;
            this.CreateUserName = createUserName;

            this.Id = Guid.NewGuid().ToString("N");

            CreateDate = DateTime.Now;
            EditDate = DateTime.Now;
            PostCover = "";
            BrowseTime = 0;
            IsClose = "F";
            Discuss = "T";
            State = 0;
        }

        public void FilterSensitiveWords(List<string> words)
        {
            foreach (var word in words)
            {
                string replacement = new string('*', word.Length);
                PostContent = PostContent.Replace(word, replacement);
                PostTitle = PostTitle.Replace(word, replacement);
                summary = summary.Replace(word, replacement);
            }
        }
    }
}
