using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.UseCases.Interfaces;
using MediatR;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventCreateCommandHandler : IRequestHandler<EventCreateCommand>
    {
        private readonly ICreateUseCaseAsync<EventDTO> _createUseCaseAsync;

        public EventCreateCommandHandler(ICreateUseCaseAsync<EventDTO> createUseCaseAsync)
        {
            _createUseCaseAsync = createUseCaseAsync;
        }

        public async Task Handle(EventCreateCommand request, CancellationToken cancellationToken)
        {
            await _createUseCaseAsync.Execute(request.Event, cancellationToken);
        }
    }
}
