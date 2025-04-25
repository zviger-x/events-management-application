using Application.MediatR.Commands;
using AutoMapper;
using Grpc.Core;
using MediatR;
using PaymentAPI.Services.Grpc.Payment;
using GrpcPaymentService = PaymentAPI.Services.Grpc.Payment.PaymentService;

namespace PaymentAPI.Services
{
    public class PaymentService : GrpcPaymentService.PaymentServiceBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public PaymentService(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        public override async Task<ProcessPaymentResponse> ProcessPayment(ProcessPaymentRequest request, ServerCallContext context)
        {
            var command = _mapper.Map<ProcessPaymentCommand>(request);

            var result = await _mediator.Send(command, context.CancellationToken);

            return new ProcessPaymentResponse { Success = result.Success };
        }
    }
}
