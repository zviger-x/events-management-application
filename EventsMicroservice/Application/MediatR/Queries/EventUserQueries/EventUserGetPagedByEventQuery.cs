using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Queries.EventUserQueries
{
    public class EventUserGetPagedByEventQuery : IRequest<PagedCollection<EventUser>>
    {
        public required Guid EventId { get; set; }
        public required PageParameters PageParameters { get; set; }
    }
}
