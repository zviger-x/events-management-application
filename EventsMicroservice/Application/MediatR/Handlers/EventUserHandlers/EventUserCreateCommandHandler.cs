using Application.MediatR.Commands.EventUserCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.MediatR.Handlers.EventUserHandlers
{
    public class EventUserCreateCommandHandler : BaseHandler<EventUser>, IRequestHandler<EventUserCreateCommand, Guid>
    {
        public EventUserCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IEventUserValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<Guid> Handle(EventUserCreateCommand request, CancellationToken cancellationToken)
        {
            var eventUser = new EventUser { EventId = request.EventId, UserId = request.UserId, RegisteredAt = DateTime.UtcNow };

            await _validator.ValidateAndThrowAsync(eventUser);

            var @event = await _unitOfWork.EventRepository.GetByIdAsync(eventUser.EventId, cancellationToken);
            if (@event == null)
                throw new ArgumentException("There is no event with this Id.", nameof(eventUser.EventId));

            // TODO: Добавить проверку наличия пользователя (gRPC запрос)

            var isRegistered = (await _unitOfWork.EventUserRepository.GetAllAsync(cancellationToken)).Any(e => e.UserId == request.UserId);
            if (isRegistered)
                throw new ArgumentException("User is already registered.");

            return await _unitOfWork.EventUserRepository.CreateAsync(eventUser, cancellationToken).ConfigureAwait(false);
        }
    }
}
