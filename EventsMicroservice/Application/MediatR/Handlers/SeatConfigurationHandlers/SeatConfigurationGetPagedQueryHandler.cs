using Application.MediatR.Queries.SeatConfigurationQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Common;
using Shared.Validation.Interfaces;
using ArgumentNullException = Shared.Exceptions.ServerExceptions.ArgumentNullException;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationGetPagedQueryHandler : BaseHandler<PageParameters>, IRequestHandler<SeatConfigurationGetPagedQuery, PagedCollection<SeatConfiguration>>
    {
        public SeatConfigurationGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IPageParametersValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<PagedCollection<SeatConfiguration>> Handle(SeatConfigurationGetPagedQuery request, CancellationToken cancellationToken)
        {
            if (request.PageParameters == null)
                throw new ArgumentNullException(nameof(request.PageParameters));

            await _validator.ValidateAndThrowAsync(request.PageParameters, cancellationToken);

            return await _unitOfWork.SeatConfigurationRepository.GetPagedAsync(
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
