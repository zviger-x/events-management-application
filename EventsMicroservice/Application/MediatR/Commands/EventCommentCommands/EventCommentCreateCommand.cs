using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands.EventCommentCommands
{
    public class EventCommentCreateCommand : IRequest<Guid>
    {
        public required Guid EventId { get; set; }
        public required CreateEventCommentDto EventComment { get; set; }
    }
}
