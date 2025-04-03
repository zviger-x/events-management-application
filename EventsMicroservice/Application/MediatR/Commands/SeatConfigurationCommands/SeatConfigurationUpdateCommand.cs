using Domain.Entities;
using MediatR;

namespace Application.MediatR.Commands.SeatConfigurationCommands
{
    public class SeatConfigurationUpdateCommand : IRequest
    {
        public SeatConfiguration SeatConfiguration { get; set; }
    }
}
