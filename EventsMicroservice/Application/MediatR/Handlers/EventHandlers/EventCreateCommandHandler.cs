using Application.Contracts;
using Application.MediatR.Commands.EventCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

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
            await _validator.ValidateAndThrowAsync(request.Event);

            var seatConfiguration = await _unitOfWork.SeatConfigurationRepository.GetByIdAsync(request.Event.SeatConfigurationId, cancellationToken);
            if (seatConfiguration == null)
                throw new ArgumentException("There is no seat configuration with this Id.");

            var @event = _mapper.Map<Event>(request.Event);
            var eventId = await _unitOfWork.EventRepository.CreateAsync(@event, cancellationToken).ConfigureAwait(false);
            await GenerateSeats(eventId, seatConfiguration, cancellationToken);

            return eventId;
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

            #warning Изменить на массовое добавление, вместо одиночного!
            foreach (var seat in seats)
               await _unitOfWork.SeatRepository.CreateAsync(seat, cancellationToken);
        }
    }
}
