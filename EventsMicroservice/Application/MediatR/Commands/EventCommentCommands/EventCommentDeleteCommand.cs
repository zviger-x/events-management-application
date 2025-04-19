using MediatR;

namespace Application.MediatR.Commands.EventCommentCommands
{
    public class EventCommentDeleteCommand : IRequest
    {
        public required Guid EventId { get; set; }
        public required Guid CommentId { get; set; }
    }
}
