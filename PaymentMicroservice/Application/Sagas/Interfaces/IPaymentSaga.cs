using Application.Contracts;
using Application.MediatR.Commands;

namespace Application.Sagas.Interfaces
{
    public interface IPaymentSaga
    {
        /// <summary>
        /// Executes the payment saga which performs the following steps:
        /// <list type="number">
        /// <item>Debits the specified amount.</item>
        /// <item>Records the transaction in the user service.</item>
        /// <item>If any step fails, compensates by refunding the amount.</item>
        /// </list>
        /// </summary>
        /// <param name="request">The payment command containing token and amount details.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="PaymentResultDto"/> indicating success or failure and any error messages.</returns>
        Task<PaymentResultDto> ExecuteAsync(ProcessPaymentCommand request, CancellationToken cancellationToken);
    }
}
