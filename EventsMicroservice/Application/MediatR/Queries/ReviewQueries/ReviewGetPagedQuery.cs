using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries.ReviewQueries
{
    public class ReviewGetPagedQuery : IRequest<PagedCollection<Review>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
