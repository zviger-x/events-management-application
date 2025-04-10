using Domain.Entities;
using MediatR;

namespace Application.MediatR.Commands.EventCommentCommands
{
    public class EventCommentUpdateCommand : IRequest
    {
        public required Guid RouteEventId { get; set; }
        public required Guid RouteCommentId { get; set; }
        public required EventComment EventComment { get; set; }
    }
}
