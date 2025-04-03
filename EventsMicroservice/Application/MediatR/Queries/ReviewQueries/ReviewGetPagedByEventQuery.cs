using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries.ReviewQueries
{
    public class ReviewGetPagedByEventQuery : IRequest<PagedCollection<Review>>
    {
        public Guid EventId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
