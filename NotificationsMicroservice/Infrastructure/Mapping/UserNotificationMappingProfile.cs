using Application.Contracts;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Shared.Grpc.User;

namespace Infrastructure.Mapping
{
    public class UserNotificationMappingProfile : Profile
    {
        public UserNotificationMappingProfile()
        {
            CreateMap<NotificationDto, CreateNotificationRequest>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.DateTime)));

            CreateMap<CreateNotificationRequest, NotificationDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime.ToDateTime()));
        }
    }
}
