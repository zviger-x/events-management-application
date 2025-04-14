using Application.MediatR.Queries.EventUserQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Interfaces;
using Shared.Common;
using Shared.Exceptions.ServerExceptions;
using Shared.Validation.Interfaces;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserGetPagedQueryHandler : BaseHandler<PageParameters>, IRequestHandler<EventUserGetPagedQuery, PagedCollection<EventUser>>
    {
        public EventUserGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, IPageParametersValidator validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task<PagedCollection<EventUser>> Handle(EventUserGetPagedQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.PageParameters, cancellationToken);

            return await _unitOfWork.EventUserRepository.GetPagedAsync(
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken);
        }
    }
}
