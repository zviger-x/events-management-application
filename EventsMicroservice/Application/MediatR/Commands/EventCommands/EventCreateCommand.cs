using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands.EventCommands
{
    public class EventCreateCommand : IRequest<Guid>
    {
        public CreateEventDTO Event { get; set; } = default!;
    }
}
