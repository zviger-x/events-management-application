using Application.MediatR.Queries.EventQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Common;
using Shared.Validation.Interfaces;
using ArgumentNullException = Shared.Exceptions.ServerExceptions.ArgumentNullException;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventGetPagedQueryHandler : BaseHandler<PageParameters>, IRequestHandler<EventGetPagedQuery, PagedCollection<Event>>
    {
        public EventGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IPageParametersValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<PagedCollection<Event>> Handle(EventGetPagedQuery request, CancellationToken cancellationToken)
        {
            if (request.PageParameters == null)
                throw new ArgumentNullException(nameof(request.PageParameters));

            await _validator.ValidateAndThrowAsync(request.PageParameters, cancellationToken);

            return await _unitOfWork.EventRepository.GetPagedAsync(
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
