using Application.Clients;
using Application.Contracts;
using AutoMapper;
using Shared.Grpc.Payment;

namespace Infrastructure.Clients.Grpc
{
    public class PaymentClient : IPaymentClient
    {
        private readonly IMapper _mapper;
        private readonly PaymentService.PaymentServiceClient _paymentServiceClient;

        public PaymentClient(IMapper mapper, PaymentService.PaymentServiceClient paymentServiceClient)
        {
            _mapper = mapper;
            _paymentServiceClient = paymentServiceClient;
        }

        public async Task<bool> ProcessPaymentAsync(ProcessPaymentDto processPaymentDto, CancellationToken cancellationToken)
        {
            var request = _mapper.Map<ProcessPaymentRequest>(processPaymentDto);

            var result = await _paymentServiceClient.ProcessPaymentAsync(request, cancellationToken: cancellationToken);

            return result.Success;
        }
    }
}
