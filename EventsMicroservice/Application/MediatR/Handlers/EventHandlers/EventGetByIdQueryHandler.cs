using Application.MediatR.Queries.EventQueries;
using Application.UseCases.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventGetByIdQueryHandler : IRequestHandler<EventGetByIdQuery, Event>
    {
        private readonly IGetByIdUseCaseAsync<Event> _getByIdUseCaseAsync;

        public EventGetByIdQueryHandler(IGetByIdUseCaseAsync<Event> getByIdUseCaseAsync)
        {
            _getByIdUseCaseAsync = getByIdUseCaseAsync;
        }

        public async Task<Event> Handle(EventGetByIdQuery request, CancellationToken cancellationToken)
        {
            return await _getByIdUseCaseAsync.Execute(request.Id, cancellationToken);
        }
    }
}
