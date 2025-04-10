using Application.MediatR.Queries.EventUserQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Common;
using Shared.Exceptions.ServerExceptions;
using Shared.Validation.Interfaces;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserGetPagedByEventQueryHandler : BaseHandler<PageParameters>, IRequestHandler<EventUserGetPagedByEventQuery, PagedCollection<EventUser>>
    {
        public EventUserGetPagedByEventQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IPageParametersValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<PagedCollection<EventUser>> Handle(EventUserGetPagedByEventQuery request, CancellationToken cancellationToken)
        {
            if (request.PageParameters == null)
                throw new ParameterNullException(nameof(request.PageParameters));

            await _validator.ValidateAndThrowAsync(request.PageParameters, cancellationToken);

            return await _unitOfWork.EventUserRepository.GetPagedByEventAsync(
                request.EventId,
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
