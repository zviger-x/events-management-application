using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries.EventUserQueries
{
    public class EventUserGetAllByEventQuery : IRequest<IEnumerable<EventUser>>
    {
        public required Guid EventId { get; set; }
    }
}
