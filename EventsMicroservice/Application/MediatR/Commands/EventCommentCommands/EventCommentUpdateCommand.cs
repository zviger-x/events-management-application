using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands.EventCommentCommands
{
    public class EventCommentUpdateCommand : IRequest
    {
        public required Guid CurrentUserId { get; set; }
        public required Guid RouteEventId { get; set; }
        public required Guid RouteCommentId { get; set; }
        public required UpdateEventCommentDto EventComment { get; set; }
    }
}
