using Application.Contracts;
using MediatR;

namespace Application.MediatR.Commands.SeatConfigurationCommands
{
    public class SeatConfigurationCreateCommand : IRequest<Guid>
    {
        public required CreateSeatConfigurationDto SeatConfiguration { get; set; }
    }
}
