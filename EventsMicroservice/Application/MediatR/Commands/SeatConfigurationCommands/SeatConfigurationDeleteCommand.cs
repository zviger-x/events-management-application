using MediatR;

namespace Application.MediatR.Commands.SeatConfigurationCommands
{
    public class SeatConfigurationDeleteCommand : IRequest
    {
        public required Guid Id { get; set; }
    }
}
