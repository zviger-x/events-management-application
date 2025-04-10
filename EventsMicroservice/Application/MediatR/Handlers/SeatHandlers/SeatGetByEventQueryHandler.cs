using Application.MediatR.Queries.SeatQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Shared.Exceptions.ServerExceptions;

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
            var seat = await _unitOfWork.SeatRepository.GetByEventAsync(request.EventId, cancellationToken);
            if (seat == null)
                throw new NotFoundException("Seat not found.");

            return seat;
        }
    }
}
