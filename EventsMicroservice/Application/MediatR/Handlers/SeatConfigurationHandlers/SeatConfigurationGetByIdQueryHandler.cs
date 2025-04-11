using Application.MediatR.Queries.SeatConfigurationQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationGetByIdQueryHandler : BaseHandler, IRequestHandler<SeatConfigurationGetByIdQuery, SeatConfiguration>
    {
        public SeatConfigurationGetByIdQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService, IMapper mapper)
            : base(unitOfWork, cacheService, mapper)
        {
        }

        public async Task<SeatConfiguration> Handle(SeatConfigurationGetByIdQuery request, CancellationToken cancellationToken)
        {
            var configuration = await _unitOfWork.SeatConfigurationRepository.GetByIdAsync(request.Id, cancellationToken);
            if (configuration == null)
                throw new NotFoundException("Configuration not found.");

            return configuration;
        }
    }
}
