using Domain.Entities;
using MediatR;

namespace Application.MediatR.Commands.EventCommands
{
    public class EventUpdateCommand : IRequest
    {
        public Event Event { get; set; } = default!;
    }
}
