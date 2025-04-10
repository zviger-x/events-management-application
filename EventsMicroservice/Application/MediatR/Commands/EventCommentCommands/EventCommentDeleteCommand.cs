using MediatR;

namespace Application.MediatR.Commands.EventCommentCommands
{
    public class EventCommentDeleteCommand : IRequest
    {
        public required Guid RouteEventId { get; set; }
        public required Guid Id { get; set; }
    }
}
