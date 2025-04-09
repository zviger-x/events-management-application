using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Queries.EventQueries
{
    public class EventGetPagedQuery : IRequest<PagedCollection<Event>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
