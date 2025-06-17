using MediatR;

namespace Application.MediatR.Commands.EventCommands
{
    public class EventDeleteCommand : IRequest
    {
        public required Guid Id { get; set; }
    }
}
