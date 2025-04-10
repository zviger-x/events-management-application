using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Queries.EventCommentQueries
{
    public class EventCommentGetPagedByEventQuery : IRequest<PagedCollection<EventComment>>
    {
        public Guid EventId { get; set; }
        public PageParameters PageParameters { get; set; }
    }
}
