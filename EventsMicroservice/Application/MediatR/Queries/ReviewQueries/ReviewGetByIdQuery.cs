using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries.ReviewQueries
{
    public class ReviewGetByIdQuery : IRequest<Review>
    {
        public Guid Id { get; set; }
    }
}
