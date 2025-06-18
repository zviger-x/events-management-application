using MediatR;

namespace Application.MediatR.Commands.EventUserCommands
{
    public class EventUserDeleteCommand : IRequest
    {
        public required Guid EventId { get; set; }
        public required Guid UserId { get; set; }
    }
}
