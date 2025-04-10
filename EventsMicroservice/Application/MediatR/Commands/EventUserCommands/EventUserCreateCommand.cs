using MediatR;

namespace Application.MediatR.Commands.EventUserCommands
{
    public class EventUserCreateCommand : IRequest<Guid>
    {
        public required Guid EventId { get; set; }
        public required Guid UserId { get; set; }
        public required Guid SeatId { get; set; }
    }
}
