using Application.MediatR.Queries.SeatQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Caching.Services.Interfaces;
using Shared.Exceptions.ServerExceptions;

namespace Application.MediatR.Handlers.SeatHandlers
{
    public class SeatGetByEventQueryHandler : BaseHandler, IRequestHandler<SeatGetByEventQuery, IEnumerable<Seat>>
    {
        public SeatGetByEventQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
            : base(unitOfWork, mapper, cacheService)
        {
        }

        public async Task<IEnumerable<Seat>> Handle(SeatGetByEventQuery request, CancellationToken cancellationToken)
        {
            var seat = await _unitOfWork.SeatRepository.GetByEventAsync(request.EventId, cancellationToken);

            return seat ?? throw new NotFoundException("Seat not found.");
        }
    }
}
