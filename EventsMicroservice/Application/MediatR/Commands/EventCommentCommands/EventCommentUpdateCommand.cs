using Domain.Entities;
using MediatR;

namespace Application.MediatR.Commands.EventCommentCommands
{
    public class EventCommentUpdateCommand : IRequest
    {
        public EventComment EventComment { get; set; }
    }
}
