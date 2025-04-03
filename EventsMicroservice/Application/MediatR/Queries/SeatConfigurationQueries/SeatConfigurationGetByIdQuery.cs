using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries.SeatConfigurationQueries
{
    public class SeatConfigurationGetByIdQuery : IRequest<SeatConfiguration>
    {
        public Guid Id { get; set; }
    }
}
