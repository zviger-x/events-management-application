using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries.SeatQueries
{
    public class SeatGetByEventQuery : IRequest<IEnumerable<Seat>>
    {
        public Guid EventId { get; set; }
    }
}
