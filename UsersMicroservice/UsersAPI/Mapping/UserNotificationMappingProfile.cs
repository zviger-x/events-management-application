using AutoMapper;
using BusinessLogic.Contracts;
using Google.Protobuf.WellKnownTypes;
using Shared.Grpc.User;

namespace UsersAPI.Mapping
{
    public class UserNotificationMappingProfile : Profile
    {
        public UserNotificationMappingProfile()
        {
            CreateMap<CreateUserNotificationDto, CreateNotificationRequest>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.DateTime)));

            CreateMap<CreateNotificationRequest, CreateUserNotificationDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime.ToDateTime()));
        }
    }
}
