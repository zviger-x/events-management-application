using AutoMapper;
using BusinessLogic.Contracts;
using DataAccess.Entities;

namespace BusinessLogic.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UpdateUserDto>();

            CreateMap<RegisterDto, User>();
            CreateMap<User, RegisterDto>();

            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Notifications, opt => opt.Condition(u => u.Notifications != null))
                .ForMember(dest => dest.Transactions, opt => opt.Condition(u => u.Transactions != null));
        }
    }
}
