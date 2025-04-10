using Domain.Entities;
using MediatR;

namespace Application.MediatR.Commands.SeatConfigurationCommands
{
    public class SeatConfigurationCreateCommand : IRequest<Guid>
    {
        public required SeatConfiguration SeatConfiguration { get; set; }
    }
}
