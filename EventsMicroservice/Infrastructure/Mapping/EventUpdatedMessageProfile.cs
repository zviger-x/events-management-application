using Application.MediatR.Commands.EventCommands;
using AutoMapper;
using Shared.Kafka.Contracts.Events;

namespace Infrastructure.Mapping
{
    public class EventUpdatedMessageProfile : Profile
    {
        public EventUpdatedMessageProfile()
        {
            CreateMap<EventUpdateCommand, EventUpdatedDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Event.Name))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
