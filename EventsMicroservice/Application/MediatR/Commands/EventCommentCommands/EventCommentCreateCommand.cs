using Domain.Entities;
using MediatR;

namespace Application.MediatR.Commands.EventCommentCommands
{
    public class EventCommentCreateCommand : IRequest<Guid>
    {
        public required Guid RouteEventId { get; set; }
        public required EventComment EventComment { get; set; }
    }
}
