using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands.EventCommands
{
    public class EventUpdateCommand : IRequest
    {
        public UpdateEventDTO Event { get; set; }
    }
}
