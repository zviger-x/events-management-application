using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Queries.EventUserQueries
{
    public class EventUserGetPagedQuery : IRequest<PagedCollection<EventUser>>
    {
        public PageParameters PageParameters { get; set; }
    }
}
