using Domain.Entities;
using MediatR;

namespace Application.MediatR.Commands.EventCommentCommands
{
    public class EventCommentCreateCommand : IRequest<Guid>
    {
        public EventComment EventComment { get; set; }
    }
}
