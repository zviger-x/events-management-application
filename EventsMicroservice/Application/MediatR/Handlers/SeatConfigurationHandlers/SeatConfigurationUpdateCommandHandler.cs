using Application.Contracts;
using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationUpdateCommandHandler : BaseHandler<UpdateSeatConfigurationDto>, IRequestHandler<SeatConfigurationUpdateCommand>
    {
        public SeatConfigurationUpdateCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService,
            IUpdateSeatConfigurationDtoValidator validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task Handle(SeatConfigurationUpdateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.SeatConfiguration, cancellationToken);

            var storedConfiguration = await _unitOfWork.SeatConfigurationRepository.GetByIdAsync(request.SeatConfigurationId, cancellationToken);
            if (storedConfiguration == null)
                throw new NotFoundException("Seat configuration not found.");

            var seatConfiguration = _mapper.Map(request.SeatConfiguration, storedConfiguration);

            await _unitOfWork.SeatConfigurationRepository.UpdateAsync(seatConfiguration, cancellationToken);
        }
    }
}
