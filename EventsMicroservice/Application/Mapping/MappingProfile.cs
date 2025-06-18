using Application.Contracts;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        // TODO: РАЗДЕЛИТЬ ПРОФИЛИ!
        public MappingProfile()
        {
            CreateMap<Event, CreateEventDto>();
            CreateMap<CreateEventDto, Event>();

            CreateMap<Event, UpdateEventDto>();
            CreateMap<UpdateEventDto, Event>();

            CreateMap<EventUser, CreateEventUserDto>();
            CreateMap<CreateEventUserDto, EventUser>();

            CreateMap<EventComment, CreateEventCommentDto>();
            CreateMap<EventComment, UpdateEventCommentDto>();
            CreateMap<CreateEventCommentDto, EventComment>();
            CreateMap<UpdateEventCommentDto, EventComment>();

            CreateMap<SeatConfiguration, CreateSeatConfigurationDto>();
            CreateMap<SeatConfiguration, UpdateSeatConfigurationDto>();
            CreateMap<CreateSeatConfigurationDto, SeatConfiguration>();
            CreateMap<UpdateSeatConfigurationDto, SeatConfiguration>();
        }
    }
}
