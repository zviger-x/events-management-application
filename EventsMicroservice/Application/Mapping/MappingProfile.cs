using Application.Contracts;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, CreateEventDTO>();
            CreateMap<CreateEventDTO, Event>();

            CreateMap<Event, UpdateEventDTO>();
            CreateMap<UpdateEventDTO, Event>();
        }
    }
}
