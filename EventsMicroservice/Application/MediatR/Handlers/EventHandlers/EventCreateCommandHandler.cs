using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using ArgumentException = Shared.Exceptions.ServerExceptions.ArgumentException;
using ArgumentNullException = Shared.Exceptions.ServerExceptions.ArgumentNullException;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventCreateCommandHandler : BaseHandler<CreateEventDTO>, IRequestHandler<EventCreateCommand, Guid>
    {
        public EventCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICreateEventDTOValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<Guid> Handle(EventCreateCommand request, CancellationToken cancellationToken)
        {
            if (request.Event == null)
                throw new ArgumentNullException(nameof(request.Event));

            await _validator.ValidateAndThrowAsync(request.Event, cancellationToken);

            var seatConfiguration = await _unitOfWork.SeatConfigurationRepository.GetByIdAsync(request.Event.SeatConfigurationId, cancellationToken);
            if (seatConfiguration == null)
                throw new ArgumentException("There is no seat configuration with this Id.");

            var @event = _mapper.Map<Event>(request.Event);
            
            return await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                var eventId = await _unitOfWork.EventRepository.CreateAsync(@event, token).ConfigureAwait(false);
                await GenerateSeats(eventId, seatConfiguration, token);

                return eventId;
            }, cancellationToken);
        }

        private async Task GenerateSeats(Guid eventId, SeatConfiguration seatConfiguration, CancellationToken cancellationToken)
        {
            var seats = new List<Seat>();

            var numberOfRows = seatConfiguration.Rows.Count;
            for (int rI = 0; rI < numberOfRows; rI++)
            {
                var numberOfSeats = seatConfiguration.Rows[rI];

                for (int i = 0; i < numberOfSeats; i++)
                {
                    seats.Add(new Seat
                    {
                        EventId = eventId,
                        Number = i + 1,
                        Row = rI + 1,
                        Price = seatConfiguration.DefaultPrice
                    });
                }
            }

            await _unitOfWork.SeatRepository.CreateManyAsync(seats, cancellationToken).ConfigureAwait(false);
        }
    }
}
