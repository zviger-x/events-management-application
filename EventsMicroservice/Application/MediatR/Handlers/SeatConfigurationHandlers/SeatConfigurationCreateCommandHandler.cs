using Application.Contracts;
using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Services.Interfaces;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationCreateCommandHandler : BaseHandler<CreateSeatConfigurationDto>, IRequestHandler<SeatConfigurationCreateCommand, Guid>
    {
        public SeatConfigurationCreateCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICacheService cacheService,
            IValidator<CreateSeatConfigurationDto> validator)
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
