using Application.Contracts;
using Application.MediatR.Commands;
using Application.Messages;
using Application.Sagas.Interfaces;
using AutoMapper;
using MediatR;
using Shared.Kafka.Contracts.Payment;

namespace Application.MediatR.Handlers
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentResultDto>
    {
        private readonly IPaymentSaga _paymentSagaOrchestrator;
        private readonly IPaymentConfirmedMessageProducer _paymentConfirmedMessageProducer;
        private readonly IMapper _mapper;

        public ProcessPaymentCommandHandler(
            IPaymentSaga paymentSagaOrchestrator,
            IPaymentConfirmedMessageProducer paymentConfirmedMessageProducer,
            IMapper mapper)
        {
            _paymentSagaOrchestrator = paymentSagaOrchestrator;
            _paymentConfirmedMessageProducer = paymentConfirmedMessageProducer;
            _mapper = mapper;
        }

        public async Task<PaymentResultDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            var result = await _paymentSagaOrchestrator.ExecuteAsync(request, cancellationToken);

            if (result.Success)
            {
                var message = _mapper.Map<PaymentConfirmedDto>(request);
                await _paymentConfirmedMessageProducer.PublishAsync(message, cancellationToken);
            }

            return result;
        }
    }
}
