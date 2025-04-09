using Application.MediatR.Queries.SeatQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.SeatHandlers
{
    public class SeatGetByEventQueryHandler : BaseHandler, IRequestHandler<SeatGetByEventQuery, IEnumerable<Seat>>
    {
        public SeatGetByEventQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<IEnumerable<Seat>> Handle(SeatGetByEventQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SeatRepository.GetByEventAsync(request.EventId, cancellationToken);
        }
    }
}
