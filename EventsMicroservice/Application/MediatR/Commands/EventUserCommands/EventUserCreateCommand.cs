using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands.EventUserCommands
{
    public class EventUserCreateCommand : IRequest<Guid>
    {
        public required CreateEventUserDto EventUser { get; set; }
    }
}
