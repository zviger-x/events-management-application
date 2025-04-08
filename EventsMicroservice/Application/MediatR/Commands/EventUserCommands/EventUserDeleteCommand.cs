using MediatR;

namespace Application.MediatR.Commands.EventUserCommands
{
    public class EventUserDeleteCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
