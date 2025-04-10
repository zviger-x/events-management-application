using Application.MediatR.Commands.SeatConfigurationCommands;
using Application.UnitOfWork.Interfaces;
using AutoMapper;
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
            var configuration = await _unitOfWork.SeatConfigurationRepository.GetByIdAsync(request.Id, cancellationToken);
            if (configuration == null)
                return;

            await _unitOfWork.SeatConfigurationRepository.DeleteAsync(configuration, cancellationToken).ConfigureAwait(false);
        }
    }
}
