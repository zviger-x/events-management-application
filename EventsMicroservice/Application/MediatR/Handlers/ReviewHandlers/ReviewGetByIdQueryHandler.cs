using Application.MediatR.Queries.ReviewQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.ReviewHandlers
{
    public class ReviewGetByIdQueryHandler : BaseHandler, IRequestHandler<ReviewGetByIdQuery, Review>
    {
        public ReviewGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<Review> Handle(ReviewGetByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ReviewRepository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
        }
    }
}
