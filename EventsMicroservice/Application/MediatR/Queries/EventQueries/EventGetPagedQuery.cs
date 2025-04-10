using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Queries.EventQueries
{
    public class EventGetPagedQuery : IRequest<PagedCollection<Event>>
    {
        public required PageParameters PageParameters { get; set; }
    }
}
