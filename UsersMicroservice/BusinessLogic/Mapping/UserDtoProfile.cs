using AutoMapper;
using BusinessLogic.Contracts;
using DataAccess.Entities;

namespace BusinessLogic.Mapping
{
    public class UserDtoProfile : Profile
    {
        public UserDtoProfile()
        {
            CreateMap<UserDto, User>();

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Notifications, opt => opt.Condition(u => u.Notifications != null))
                .ForMember(dest => dest.Transactions, opt => opt.Condition(u => u.Transactions != null));
        }
    }
}
