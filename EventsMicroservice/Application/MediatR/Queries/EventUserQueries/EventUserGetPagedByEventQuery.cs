using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Queries.EventUserQueries
{
    public class EventUserGetPagedByEventQuery : IRequest<PagedCollection<EventUser>>
    {
        public Guid EventId { get; set; }
        public PageParameters PageParameters { get; set; }
    }
}
