using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Queries.EventCommentQueries
{
    public class EventCommentGetPagedByEventQuery : IRequest<PagedCollection<EventComment>>
    {
        public required Guid EventId { get; set; }
        public required PageParameters PageParameters { get; set; }
    }
}
