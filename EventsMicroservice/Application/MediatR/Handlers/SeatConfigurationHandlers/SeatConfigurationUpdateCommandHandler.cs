using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

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
            await _validator.ValidateAndThrowAsync(request.SeatConfiguration);

            await _unitOfWork.SeatConfigurationRepository.UpdateAsync(request.SeatConfiguration, cancellationToken).ConfigureAwait(false);
        }
    }
}
