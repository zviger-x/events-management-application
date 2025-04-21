using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationUpdateCommandHandler : BaseHandler, IRequestHandler<SeatConfigurationUpdateCommand>
    {
        public SeatConfigurationUpdateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task Handle(SeatConfigurationUpdateCommand request, CancellationToken cancellationToken)
        {
            var storedConfiguration = await _unitOfWork.SeatConfigurationRepository.GetByIdAsync(request.SeatConfigurationId, cancellationToken);
            if (storedConfiguration == null)
                throw new NotFoundException("Seat configuration not found.");

            var seatConfiguration = _mapper.Map(request.SeatConfiguration, storedConfiguration);

            await _unitOfWork.SeatConfigurationRepository.UpdateAsync(seatConfiguration, cancellationToken);
        }
    }
}
