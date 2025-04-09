using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Queries.EventCommentQueries
{
    public class EventCommentGetPagedQuery : IRequest<PagedCollection<EventComment>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
