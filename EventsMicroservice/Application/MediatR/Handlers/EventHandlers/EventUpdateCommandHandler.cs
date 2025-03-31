using Application.MediatR.Commands.EventCommands;
using Application.UseCases.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventUpdateCommandHandler : IRequestHandler<EventUpdateCommand>
    {
        private readonly IUpdateUseCaseAsync<Event> _updateUseCaseAsync;

        public EventUpdateCommandHandler(IUpdateUseCaseAsync<Event> updateUseCaseAsync)
        {
            _updateUseCaseAsync = updateUseCaseAsync;
        }

        public async Task Handle(EventUpdateCommand request, CancellationToken cancellationToken)
        {
            await _updateUseCaseAsync.Execute(request.Event, cancellationToken);
        }
    }
}
