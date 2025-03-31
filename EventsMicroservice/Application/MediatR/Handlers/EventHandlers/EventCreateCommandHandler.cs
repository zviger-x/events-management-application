using Application.MediatR.Commands.EventCommands;
using Application.UseCases.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventCreateCommandHandler : IRequestHandler<EventCreateCommand>
    {
        private readonly ICreateUseCaseAsync<Event> _createUseCaseAsync;

        public EventCreateCommandHandler(ICreateUseCaseAsync<Event> createUseCaseAsync)
        {
            _createUseCaseAsync = createUseCaseAsync;
        }

        public async Task Handle(EventCreateCommand request, CancellationToken cancellationToken)
        {
            await _createUseCaseAsync.Execute(request.Event, cancellationToken);
        }
    }
}
