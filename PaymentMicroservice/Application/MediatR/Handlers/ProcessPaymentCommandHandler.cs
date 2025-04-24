using Application.Clients;
using Application.Contracts;
using Application.MediatR.Commands;
using AutoMapper;
using MediatR;

namespace Application.MediatR.Handlers
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentResultDto>
    {
        private readonly IMapper _mapper;
        private readonly IUserClient _userClient;
        private readonly IPaymentClient _paymentClient;

        public ProcessPaymentCommandHandler(IMapper mapper, IUserClient userClient, IPaymentClient paymentClient)
        {
            _mapper = mapper;
            _userClient = userClient;
            _paymentClient = paymentClient;
        }

        public async Task<PaymentResultDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            // Оплачиваем
            var success = await _paymentClient.ProcessPaymentAsync(request.Token, request.Amount);
            if (!success)
                return new PaymentResultDto { Success = success };

            var transaction = _mapper.Map<CreateUserTransactionDto>(request);

            // Сохраняем транзакцию в истории
            await _userClient.CreateTransactionAsync(transaction);

            // Возвращаем успешный статус
            return new PaymentResultDto { Success = success };
        }
    }
}
