using AutoMapper;
using BusinessLogic.Contracts;
using DataAccess.Entities;

namespace BusinessLogic.Mapping
{
    public class UserNotificationProfile : Profile
    {
        public UserNotificationProfile()
        {
            CreateMap<CreateUserNotificationDto, UserNotification>();
            CreateMap<UserNotification, CreateUserNotificationDto>();

            CreateMap<UpdateUserNotificationDto, UserNotification>();
            CreateMap<UserNotification, UpdateUserNotificationDto>();
        }
    }
}
