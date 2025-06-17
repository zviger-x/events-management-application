using Application.MediatR.Commands.EventCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.EventHandlers
{
    public class EventCreateCommandHandler : BaseHandler, IRequestHandler<EventCreateCommand, Guid>
    {
        public EventCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<Guid> Handle(EventCreateCommand request, CancellationToken cancellationToken)
        {
            var seatConfiguration = await _unitOfWork.SeatConfigurationRepository.GetByIdAsync(request.Event.SeatConfigurationId, cancellationToken);
            if (seatConfiguration == null)
                throw new ParameterException("There is no seat configuration with this Id.");

            var @event = _mapper.Map<Event>(request.Event);

            return await _unitOfWork.InvokeWithTransactionAsync(async (token) =>
            {
                var eventId = await _unitOfWork.EventRepository.CreateAsync(@event, token);
                await GenerateSeats(eventId, seatConfiguration, token);

                return eventId;
            }, cancellationToken);
        }

        private async Task GenerateSeats(Guid eventId, SeatConfiguration seatConfiguration, CancellationToken cancellationToken)
        {
            var seats = new List<Seat>();

            var numberOfRows = seatConfiguration.Rows.Count;
            for (int rowNumber = 1; rowNumber <= numberOfRows; rowNumber++)
            {
                var numberOfSeats = seatConfiguration.Rows[rowNumber - 1];

                for (int seatNumber = 1; seatNumber <= numberOfSeats; seatNumber++)
                {
                    seats.Add(new Seat
                    {
                        EventId = eventId,
                        Number = seatNumber,
                        Row = rowNumber,
                        Price = seatConfiguration.DefaultPrice
                    });
                }
            }

            await _unitOfWork.SeatRepository.CreateManyAsync(seats, cancellationToken);
        }
    }
}
