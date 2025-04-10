using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationUpdateCommandHandler : BaseHandler<SeatConfiguration>, IRequestHandler<SeatConfigurationUpdateCommand>
    {
        public SeatConfigurationUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ISeatConfigurationValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task Handle(SeatConfigurationUpdateCommand request, CancellationToken cancellationToken)
        {
            if (request.SeatConfiguration == null)
                throw new ParameterNullException(nameof(request.SeatConfiguration));

            if (request.RouteSeatId != request.SeatConfiguration.Id)
                throw new ParameterException("You are not allowed to modify this configuration.");

            await _validator.ValidateAndThrowAsync(request.SeatConfiguration, cancellationToken);

            await _unitOfWork.SeatConfigurationRepository.UpdateAsync(request.SeatConfiguration, cancellationToken).ConfigureAwait(false);
        }
    }
}
