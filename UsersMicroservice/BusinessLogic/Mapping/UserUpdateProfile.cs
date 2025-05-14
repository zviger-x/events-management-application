using AutoMapper;
using BusinessLogic.Contracts;
using DataAccess.Entities;

namespace BusinessLogic.Mapping
{
    public class UserUpdateProfile : Profile
    {
        public UserUpdateProfile()
        {
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UpdateUserDto>();
        }
    }
}
