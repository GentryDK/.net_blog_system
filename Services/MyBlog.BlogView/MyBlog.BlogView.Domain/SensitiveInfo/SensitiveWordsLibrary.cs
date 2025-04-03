using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace MyBlog.BlogSystem.Domain.SensitiveInfo
{
    /// <summary>
    /// 敏感词库
    /// </summary>
    public class SensitiveWordsLibrary : Entity<string>, IHasCreationTime
    {
        public SensitiveWordsLibrary()
        {

        }

        public SensitiveWordsLibrary(string id) : base(id)
        {

        }

        /// <summary>
        /// 敏感词库的名称
        /// </summary>
        public string LibrartFileName { get; set; }

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 文件上传路径
        /// </summary>
        public string LibraryFileUrl { get; set; }

        /// <summary>
        /// 敏感词库的开启和关闭状态
        /// </summary>
        public SenstiveStatus Statue { get; set; }

        public enum SenstiveStatus
        {
            Disable,
            Enable,
        }

        public void InitData(string libraryFileUrl, string librartFileName)
        {
            Statue = SenstiveStatus.Enable;
            LibraryFileUrl = libraryFileUrl;
            LibrartFileName = librartFileName;
        }
    }
}
