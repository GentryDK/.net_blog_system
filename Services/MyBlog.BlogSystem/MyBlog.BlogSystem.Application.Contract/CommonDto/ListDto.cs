using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Application.Contract.CommonDto
{
    public class ListDto<T>
    {
        public int Count { get; set; }

        //这里的T表示实际需要穿出去的数据
        public List<T> list { get; set; }
    }
}
