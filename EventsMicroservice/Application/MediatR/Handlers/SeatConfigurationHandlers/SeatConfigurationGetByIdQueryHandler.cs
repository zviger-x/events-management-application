using Application.MediatR.Queries.SeatConfigurationQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationGetByIdQueryHandler : BaseHandler, IRequestHandler<SeatConfigurationGetByIdQuery, SeatConfiguration>
    {
        public SeatConfigurationGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<SeatConfiguration> Handle(SeatConfigurationGetByIdQuery request, CancellationToken cancellationToken)
        {
            var configuration = await _unitOfWork.SeatConfigurationRepository.GetByIdAsync(request.Id, cancellationToken);
            return configuration ?? throw new NotFoundException("Configuration not found.");
        }
    }
}
