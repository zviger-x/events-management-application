using Application.MediatR.Queries.SeatConfigurationQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Common;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationGetPagedQueryHandler : BaseHandler, IRequestHandler<SeatConfigurationGetPagedQuery, PagedCollection<SeatConfiguration>>
    {
        public SeatConfigurationGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<PagedCollection<SeatConfiguration>> Handle(SeatConfigurationGetPagedQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageNumber));

            if (request.PageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(request.PageSize));

            return await _unitOfWork.SeatConfigurationRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken).ConfigureAwait(false);
        }
    }
}
