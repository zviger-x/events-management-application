using Application.MediatR.Queries.EventCommentQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Common;
using Shared.Exceptions.ServerExceptions;
using Shared.Validation.Interfaces;

namespace Application.MediatR.Handlers.EventCommentHandlers
{
    public class EventCommentGetPagedByEventQueryHandler : BaseHandler<PageParameters>, IRequestHandler<EventCommentGetPagedByEventQuery, PagedCollection<EventComment>>
    {
        public EventCommentGetPagedByEventQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IPageParametersValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<PagedCollection<EventComment>> Handle(EventCommentGetPagedByEventQuery request, CancellationToken cancellationToken)
        {
            if (request.PageParameters == null)
                throw new ParameterNullException(nameof(request.PageParameters));

            await _validator.ValidateAndThrowAsync(request.PageParameters, cancellationToken);

            return await _unitOfWork.EventCommentRepository.GetPagedByEventAsync(
                request.EventId,
                request.PageParameters.PageNumber,
                request.PageParameters.PageSize,
                cancellationToken)
                ;
        }
    }
}
