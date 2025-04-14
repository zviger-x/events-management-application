using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.UnitOfWork.Interfaces;
using Application.Validation.Validators.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Shared.Caching.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationCreateCommandHandler : BaseHandler<SeatConfiguration>, IRequestHandler<SeatConfigurationCreateCommand, Guid>
    {
        public SeatConfigurationCreateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService, ISeatConfigurationValidator validator)
            : base(unitOfWork, mapper, cacheService, validator)
        {
        }

        public async Task<Guid> Handle(SeatConfigurationCreateCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request.SeatConfiguration, cancellationToken);

            return await _unitOfWork.SeatConfigurationRepository.CreateAsync(request.SeatConfiguration, cancellationToken);
        }
    }
}
