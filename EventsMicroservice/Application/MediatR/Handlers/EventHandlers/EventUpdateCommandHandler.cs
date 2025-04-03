using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.UseCases.Interfaces;
using MediatR;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventUpdateCommandHandler : IRequestHandler<EventUpdateCommand>
    {
        private readonly IUpdateUseCaseAsync<UpdateEventDTO> _updateUseCaseAsync;

        public EventUpdateCommandHandler(IUpdateUseCaseAsync<UpdateEventDTO> updateUseCaseAsync)
        {
            _updateUseCaseAsync = updateUseCaseAsync;
        }

        public async Task Handle(EventUpdateCommand request, CancellationToken cancellationToken)
        {
            await _updateUseCaseAsync.Execute(request.Event, cancellationToken);
        }
    }
}
