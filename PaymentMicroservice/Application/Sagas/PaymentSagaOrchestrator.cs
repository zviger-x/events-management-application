using Application.Clients;
using Application.Contracts;
using Application.MediatR.Commands;
using Application.Sagas.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Sagas
{
    public class PaymentSagaOrchestrator : IPaymentSaga
    {
        private readonly IMapper _mapper;
        private readonly IUserClient _userClient;
        private readonly IPaymentClient _paymentClient;
        private readonly ILogger<PaymentSagaOrchestrator> _logger;

        public PaymentSagaOrchestrator(IMapper mapper, IUserClient userClient, IPaymentClient paymentClient, ILogger<PaymentSagaOrchestrator> logger)
        {
            _mapper = mapper;
            _userClient = userClient;
            _paymentClient = paymentClient;
            _logger = logger;
        }

        public async Task<PaymentResultDto> ExecuteAsync(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            // Списываем деньги (если внутренняя ошибка - её словит gRPC interceptor)
            if (!await TryProcessPaymentAsync(request, cancellationToken))
                return Failure();

            // Попытка сохранить транзакцию и завершить сагу
            if (await TrySaveTransactionAsync(request, cancellationToken))
                return Success();

            // Если сохранить не получилось - возвращаем деньги
            await CompensateAsync(request);
            return Failure();
        }

        private async Task<bool> TryProcessPaymentAsync(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            return await _paymentClient.ProcessPaymentAsync(request.Token, request.Amount, cancellationToken);
        }

        private async Task<bool> TrySaveTransactionAsync(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var transaction = _mapper.Map<CreateUserTransactionDto>(request);
                await _userClient.CreateTransactionAsync(transaction, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving transaction.");

                return false;
            }
        }

        private async Task CompensateAsync(ProcessPaymentCommand request)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

                var refundSuccess = await _paymentClient.RefundPaymentAsync(request.Token, request.Amount, cts.Token);

                if (!refundSuccess)
                    _logger.LogCritical("Failed to execute RefundPaymentAsync after transaction failed.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Refund error.");
            }
        }

        private PaymentResultDto Success() => new PaymentResultDto { Success = true };

        private PaymentResultDto Failure() => new PaymentResultDto { Success = false };
    }
}
