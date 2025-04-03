using Application.MediatR.Queries.SeatConfigurationQueries;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationGetPagedQueryHandler : BaseHandler, IRequestHandler<SeatConfigurationGetPagedQuery, PagedCollection<SeatConfiguration>>
    {
        public SeatConfigurationGetPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task<PagedCollection<SeatConfiguration>> Handle(SeatConfigurationGetPagedQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SeatConfigurationRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken).ConfigureAwait(false);
        }
    }
}
