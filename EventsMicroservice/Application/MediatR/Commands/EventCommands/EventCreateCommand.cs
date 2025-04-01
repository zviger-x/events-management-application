using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands.EventCommands
{
    public class EventCreateCommand : IRequest
    {
        public EventDTO Event { get; set; } = default!;
    }
}
