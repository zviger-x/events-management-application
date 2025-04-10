using MediatR;

namespace Application.MediatR.Commands.EventUserCommands
{
    public class EventUserDeleteCommand : IRequest
    {
        public required Guid Id { get; set; }
    }
}
