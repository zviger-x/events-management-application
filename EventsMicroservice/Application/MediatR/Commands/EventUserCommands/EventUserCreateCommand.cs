using MediatR;

namespace Application.MediatR.Commands.EventUserCommands
{
    public class EventUserCreateCommand : IRequest<Guid>
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
    }
}
