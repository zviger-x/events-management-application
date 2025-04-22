using AutoMapper;
using BusinessLogic.Contracts;
using DataAccess.Entities;

namespace BusinessLogic.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UpdateUserDto>();

            CreateMap<RegisterDto, User>();
            CreateMap<User, RegisterDto>();
        }
    }
}
