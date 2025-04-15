using Application.Contracts;
using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Interfaces;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationCreateCommandHandler : BaseHandler<CreateSeatConfigurationDto>, IRequestHandler<SeatConfigurationCreateCommand, Guid>
    {
        public SeatConfigurationCreateCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService,
            ICreateSeatConfigurationDtoValidator validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task<Guid> Handle(SeatConfigurationCreateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.SeatConfiguration, cancellationToken);

            var seatConfiguration = _mapper.Map<SeatConfiguration>(request.SeatConfiguration);

            return await _unitOfWork.SeatConfigurationRepository.CreateAsync(seatConfiguration, cancellationToken);
        }
    }
}
