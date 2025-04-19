using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands.EventCommands
{
    public class EventUpdateCommand : IRequest
    {
        public required Guid EventId { get; set; }
        public required UpdateEventDto Event { get; set; }
    }
}
