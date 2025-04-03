using Application.MediatR.Queries.ReviewQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.ReviewHandlers
{
    public class ReviewGetPagedByEventQueryHandler : BaseHandler, IRequestHandler<ReviewGetPagedByEventQuery, PagedCollection<Review>>
    {
        public ReviewGetPagedByEventQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<PagedCollection<Review>> Handle(ReviewGetPagedByEventQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageNumber));

            if (request.PageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageSize));

            return await _unitOfWork.ReviewRepository.GetPagedByEventAsync(request.EventId, request.PageNumber, request.PageSize, cancellationToken).ConfigureAwait(false);
        }
    }
}
