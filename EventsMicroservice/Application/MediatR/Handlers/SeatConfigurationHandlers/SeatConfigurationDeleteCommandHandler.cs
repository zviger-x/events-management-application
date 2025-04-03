using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.MediatR.Handlers.SeatConfigurationHandlers
{
    public class SeatConfigurationDeleteCommandHandler : BaseHandler, IRequestHandler<SeatConfigurationDeleteCommand>
    {
        public SeatConfigurationDeleteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        public async Task Handle(SeatConfigurationDeleteCommand request, CancellationToken cancellationToken)
        {
            var configuration = new SeatConfiguration { Id = request.Id };

            await _unitOfWork.SeatConfigurationRepository.DeleteAsync(configuration, cancellationToken).ConfigureAwait(false);
        }
    }
}
