using Application.MediatR.Queries.EventCommentQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Interfaces;
using Shared.Common;
using Shared.Validation.Interfaces;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentGetPagedQueryHandler : BaseHandler<PageParameters>, IRequestHandler<EventCommentGetPagedQuery, PagedCollection<EventComment>>
    {
        public EventCommentGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, IPageParametersValidator validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task<PagedCollection<EventComment>> Handle(EventCommentGetPagedQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.PageParameters, cancellationToken);

            return await _unitOfWork.EventCommentRepository.GetPagedAsync(
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken);
        }
    }
}
