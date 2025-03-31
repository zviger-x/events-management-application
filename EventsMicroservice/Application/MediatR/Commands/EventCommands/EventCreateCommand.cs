using Domain.Entities;
using MediatR;

namespace Application.MediatR.Commands.EventCommands
{
    public class EventCreateCommand : IRequest
    {
        public Event Event { get; set; } = default!;
    }
}
