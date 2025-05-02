using Application.Contracts;
using Application.MediatR.Commands;
using Application.Sagas;
using MediatR;

namespace Application.MediatR.Handlers
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentResultDto>
    {
        private readonly PaymentSagaOrchestrator _paymentSagaOrchestrator;

        public ProcessPaymentCommandHandler(PaymentSagaOrchestrator paymentSagaOrchestrator)
        {
            _paymentSagaOrchestrator = paymentSagaOrchestrator;
        }

        public async Task<PaymentResultDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            return await _paymentSagaOrchestrator.ExecuteAsync(request, cancellationToken);
        }
    }
}
