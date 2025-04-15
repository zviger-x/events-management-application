using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands.SeatConfigurationCommands
{
    public class SeatConfigurationUpdateCommand : IRequest
    {
        public required Guid RouteSeatId { get; set; }
        public required UpdateSeatConfigurationDto SeatConfiguration { get; set; }
    }
}
