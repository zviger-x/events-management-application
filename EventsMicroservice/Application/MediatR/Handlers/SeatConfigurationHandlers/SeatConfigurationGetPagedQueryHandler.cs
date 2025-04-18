using Application.MediatR.Queries.SeatConfigurationQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Common;
using Shared.Validation.Interfaces;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationGetPagedQueryHandler : BaseHandler<PageParameters>, IRequestHandler<SeatConfigurationGetPagedQuery, PagedCollection<SeatConfiguration>>
    {
        public SeatConfigurationGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, IPageParametersValidator validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task<PagedCollection<SeatConfiguration>> Handle(SeatConfigurationGetPagedQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.PageParameters, cancellationToken);

            return await _unitOfWork.SeatConfigurationRepository.GetPagedAsync(
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken);
        }
    }
}
