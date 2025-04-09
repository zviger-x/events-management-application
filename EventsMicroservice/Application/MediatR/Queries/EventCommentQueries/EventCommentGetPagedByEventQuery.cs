using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Queries.EventCommentQueries
{
    public class EventCommentGetPagedByEventQuery : IRequest<PagedCollection<EventComment>>
    {
        public Guid EventId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
