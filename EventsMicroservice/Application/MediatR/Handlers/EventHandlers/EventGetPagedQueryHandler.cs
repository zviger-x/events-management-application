using Application.MediatR.Queries.EventQueries;
using Application.UseCases.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventGetPagedQueryHandler : IRequestHandler<EventGetPagedQuery, PagedCollection<Event>>
    {
        private readonly IGetPagedUseCaseAsync<Event> _getPagedUseCaseAsync;

        public EventGetPagedQueryHandler(IGetPagedUseCaseAsync<Event> getPagedUseCaseAsync)
        {
            _getPagedUseCaseAsync = getPagedUseCaseAsync;
        }

        public async Task<PagedCollection<Event>> Handle(EventGetPagedQuery request, CancellationToken cancellationToken)
        {
            return await _getPagedUseCaseAsync.Execute(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
