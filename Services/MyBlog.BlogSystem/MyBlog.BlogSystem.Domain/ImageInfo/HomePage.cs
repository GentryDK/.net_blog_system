using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace MyBlog.BlogSystem.Domain.ImageInfo
{
    /// <summary>
    /// 首页的图片
    /// </summary>
    public class HomePage:Entity<string>
    {
        public HomePage(string id):base(id) 
        {

        }

        /// <summary>
        /// 首页图片地址
        /// </summary>
        public string HomePageImgeUrl {  get; set; }

        /// <summary>
        /// 首页图片是否被删除
        /// </summary>
        public string IsDeleted { get; set; }
    }
}
