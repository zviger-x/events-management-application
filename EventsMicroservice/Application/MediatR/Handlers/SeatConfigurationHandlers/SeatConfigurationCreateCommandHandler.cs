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
    public class SeatConfigurationCreateCommandHandler : BaseHandler<SeatConfiguration>, IRequestHandler<SeatConfigurationCreateCommand, Guid>
    {
        public SeatConfigurationCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ISeatConfigurationValidator validator)
            : base(unitOfWork, mapper, validator)
        {
        }

        public async Task<Guid> Handle(SeatConfigurationCreateCommand request, CancellationToken cancellationToken)
        {
            if (request.SeatConfiguration == null)
                throw new ParameterNullException(nameof(request.SeatConfiguration));

            await _validator.ValidateAndThrowAsync(request.SeatConfiguration, cancellationToken);

            return await _unitOfWork.SeatConfigurationRepository.CreateAsync(request.SeatConfiguration, cancellationToken).ConfigureAwait(false);
        }
    }
}
