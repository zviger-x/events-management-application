using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Queries.EventQueries
{
    public class EventGetPagedQuery : IRequest<PagedCollection<Event>>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Location { get; set; }
        public required DateTime? FromDate { get; set; }
        public required DateTime? ToDate { get; set; }
        public required PageParameters PageParameters { get; set; }
    }
}
