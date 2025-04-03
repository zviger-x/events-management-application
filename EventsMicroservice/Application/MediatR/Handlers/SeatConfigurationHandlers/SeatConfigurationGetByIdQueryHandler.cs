using Application.MediatR.Queries.SeatConfigurationQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationGetByIdQueryHandler : BaseHandler, IRequestHandler<SeatConfigurationGetByIdQuery, SeatConfiguration>
    {
        public SeatConfigurationGetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<SeatConfiguration> Handle(SeatConfigurationGetByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SeatConfigurationRepository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
        }
    }
}
