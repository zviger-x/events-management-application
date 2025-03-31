using Application.MediatR.Commands.EventCommands;
using Application.UseCases.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventDeleteCommandHandler : IRequestHandler<EventDeleteCommand>
    {
        private readonly IDeleteUseCaseAsync<Event> _deleteUseCaseAsync;

        public EventDeleteCommandHandler(IDeleteUseCaseAsync<Event> deleteUseCaseAsync)
        {
            _deleteUseCaseAsync = deleteUseCaseAsync;
        }

        public async Task Handle(EventDeleteCommand request, CancellationToken cancellationToken)
        {
            await _deleteUseCaseAsync.Execute(request.Id, cancellationToken);
        }
    }
}
