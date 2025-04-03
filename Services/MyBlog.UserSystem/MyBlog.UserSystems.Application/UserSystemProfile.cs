using AutoMapper;
using MyBlog.UserSystem.Application.Contract.RoleApp.Dtos;
using MyBlog.UserSystem.Application.Contract.UserApp.Dtos;
using MyBlog.UserSystem.Domain.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.UserSystem.Application
{
    public class UserSystemProfile:Profile
    {
        public UserSystemProfile() {

            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString("N")))
                .ForMember(dest => dest.Role, opt => opt.Ignore()) 
                .ForMember(dest => dest.UserPassword, opt => opt.Ignore());

            CreateMap<RoleDto, Role>();
            CreateMap<Role, RoleDto>();
            //ForAllMembers 是一种全局配置方法，用于对映射中的所有成员应用相同的配置或规则。
            //在这里，它与 Condition 方法结合使用，为所有成员添加一个条件
            //src：源对象，在此上下文中为 UserInfoDto。
            //dest：目标对象，在此上下文中为 User。
            //srcMember：源成员，即 UserInfoDto 中的属性值

            //条件 (src, dest, srcMember) => srcMember != null：
            //只有在 srcMember（即 UserInfoDto 中的属性值）不为 null 时，才进行映射。
            //如果 srcMember 为 null，那么目标对象（User）的对应属性将保持原样，不会被 null 覆盖
            //CreateMap<UserUpdateTransferDto, User>().ForAllMembers(opt => 
            //    opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<User, UserInfoDto>().ForMember(dest=>dest.UserId,opt=>opt.MapFrom(src=>src.Id));

            CreateMap<List<User>, List<UserInfoDto>>().ConvertUsing((src, dest, context) =>
            {
                return src.Select(user => context.Mapper.Map<UserInfoDto>(user)).ToList();
            });
        }
    }
}
