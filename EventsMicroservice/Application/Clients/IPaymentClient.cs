using Application.Contracts;

namespace Application.Clients
{
    public interface IPaymentClient
    {
        /// <summary>
        /// Processes the payment request and returns a boolean indicating whether the transaction was successful.
        /// </summary>
        /// <param name="processPaymentDto">The data transfer object containing payment details.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>True if the payment was processed successfully; otherwise, false.</returns>
        Task<bool> ProcessPaymentAsync(ProcessPaymentDto processPaymentDto, CancellationToken cancellationToken);
    }
}
