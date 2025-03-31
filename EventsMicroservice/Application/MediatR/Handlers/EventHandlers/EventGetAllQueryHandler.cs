using Application.MediatR.Queries.EventQueries;
using Application.UseCases.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventGetAllQueryHandler : IRequestHandler<EventGetAllQuery, IEnumerable<Event>>
    {
        private readonly IGetAllUseCaseAsync<Event> _getAllUseCaseAsync;

        public EventGetAllQueryHandler(IGetAllUseCaseAsync<Event> getAllUseCaseAsync)
        {
            _getAllUseCaseAsync = getAllUseCaseAsync;
        }

        public async Task<IEnumerable<Event>> Handle(EventGetAllQuery request, CancellationToken cancellationToken)
        {
            return await _getAllUseCaseAsync.Execute(cancellationToken);
        }
    }
}
