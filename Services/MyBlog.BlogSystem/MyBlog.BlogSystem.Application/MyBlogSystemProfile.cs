using AutoMapper;
using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto;
using MyBlog.BlogSystem.Application.Contract.ReplyApp.Dto;
using MyBlog.BlogSystem.Application.Contract.UserApp.Dto;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogSystem.Domain.Shared.Redis;
using MyBlog.BlogSystem.Domain.Shared.Redis.Dtos;
using MyBlog.UserSystem.Application.Contract.UserApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogSystem.Application
{
    public class MyBlogSystemProfile : Profile
    {
        public MyBlogSystemProfile()
        {
            //des：目标对象（PostDto）中的属性 PostTypeName。
            //opt：提供选项来配置映射规则。
            //MapFrom(src => src.PostType)：指定源对象（Post）中的 PostType 属性的值用于映射到目标对象的 PostTypeName 属性。
            //意思：将 Post 类中的 PostType 属性映射到 PostDto 类中的 PostTypeName 属性。
            CreateMap<Post, PostDto>();

            CreateMap<Reply, ReplyDto>();

            CreateMap<PostType, PostTypeDto>();

            CreateMap<PostCreateDto, Post>();

            CreateMap<PostTypeDto, PostType>();

            CreateMap<PostTypeDto, PostType>();

            CreateMap<PostDaftDto, PostDraft>();

            CreateMap<PostDaftDto, PostDto>();

            CreateMap<PostDraft, PostDto>();
        }
    }
}
