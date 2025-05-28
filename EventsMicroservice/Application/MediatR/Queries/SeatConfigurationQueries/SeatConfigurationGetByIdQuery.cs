using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries.SeatConfigurationQueries
{
    public class SeatConfigurationGetByIdQuery : IRequest<SeatConfiguration>
    {
        public required Guid Id { get; set; }
    }
}
