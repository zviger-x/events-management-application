using Application.Contracts;
using Application.MediatR.Commands;
using Application.Sagas.Interfaces;
using MediatR;

namespace Application.MediatR.Handlers
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentResultDto>
    {
        private readonly IPaymentSaga _paymentSagaOrchestrator;

        public ProcessPaymentCommandHandler(IPaymentSaga paymentSagaOrchestrator)
        {
            _paymentSagaOrchestrator = paymentSagaOrchestrator;
        }

        public async Task<PaymentResultDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            return await _paymentSagaOrchestrator.ExecuteAsync(request, cancellationToken);
        }
    }
}
