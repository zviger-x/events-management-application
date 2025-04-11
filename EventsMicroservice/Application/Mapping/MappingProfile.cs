using Application.Contracts;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, CreateEventDto>();
            CreateMap<CreateEventDto, Event>();

            CreateMap<Event, UpdateEventDto>();
            CreateMap<UpdateEventDto, Event>();
        }
    }
}
