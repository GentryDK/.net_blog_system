using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto
{
    public class PostTypeDto : EntityDto<string>
    {
        public string Cover { get; set; }

        /// <summary>
        /// 板块名称
        /// </summary>
        public string PostTypeName { get; set; }

        /// <summary>
        /// 板块是否被删除
        /// </summary>
        public string IsDeleted { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string TypeBrief {  get; set; }

        /// <summary>
        /// 当前类型下帖子的数量
        /// </summary>
        public int Count { get; set; }

        public PostTypeDto() { }

        public PostTypeDto(
            string? postTypeId,
            string postTypeName,
            string typeBrief
            )
        {
            if (postTypeId != null)
            {
                this.Id = postTypeId;
            }
            this.PostTypeName = postTypeName;
            this.TypeBrief = typeBrief;
        }
    }
}
