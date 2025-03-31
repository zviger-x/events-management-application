using MediatR;

namespace Application.MediatR.Commands.EventCommands
{
    public class EventDeleteCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
