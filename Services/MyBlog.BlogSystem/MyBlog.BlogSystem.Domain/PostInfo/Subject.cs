using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace MyBlog.BlogSystem.Domain.PostInfo
{
    /// <summary>
    /// 专题
    /// </summary>
    public class Subject : Entity<string>
    {
        public Subject(string id) : base(id)
        {

        }

        /// <summary>
        /// 专题名称
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// 专题是否被删除
        /// </summary>
        public string IsDeleted { get; set; }
    }
}
