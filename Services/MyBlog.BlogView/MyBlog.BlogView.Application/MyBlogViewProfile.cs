using AutoMapper;
using AutoMapper.Internal.Mappers;
using MyBlog.BlogSystem.Application.Contract.PostApp.Dto;
using MyBlog.BlogSystem.Application.Contract.PostTypeApp.Dto;
using MyBlog.BlogSystem.Domain.PostInfo;
using MyBlog.BlogView.Application.Contract.PostApp.Dto;
using MyBlog.BlogView.Application.Contract.ReplyApp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BlogView.Application
{
    public class MyBlogViewProfile : Profile
    {
        public MyBlogViewProfile()
        {
            CreateMap<Post, PostDto>();
            CreateMap<Reply, ReplyDto>();
            CreateMap<ReplyDto,Reply>();
            CreateMap<Post, PostDetailDto>();
            CreateMap<PostType, PostTypeDto>();
        }
    }
}
