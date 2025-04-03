using MediatR;

namespace Application.MediatR.Commands.EventCommentCommands
{
    public class EventCommentDeleteCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
