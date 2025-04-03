using MediatR;

namespace Application.MediatR.Commands.SeatConfigurationCommands
{
    public class SeatConfigurationDeleteCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
