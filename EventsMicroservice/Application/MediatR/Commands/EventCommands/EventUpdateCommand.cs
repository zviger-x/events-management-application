using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands.EventCommands
{
    public class EventUpdateCommand : IRequest
    {
        public required Guid RouteEventId { get; set; }
        public required UpdateEventDTO Event { get; set; }
    }
}
