using Application.MediatR.Queries.SeatConfigurationQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Common;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationGetPagedQueryHandler : BaseHandler, IRequestHandler<SeatConfigurationGetPagedQuery, PagedCollection<SeatConfiguration>>
    {
        public SeatConfigurationGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<PagedCollection<SeatConfiguration>> Handle(SeatConfigurationGetPagedQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SeatConfigurationRepository.GetPagedAsync(
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken);
        }
    }
}
