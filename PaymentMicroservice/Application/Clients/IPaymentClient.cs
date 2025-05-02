namespace Application.Clients
{
    /// <summary>
    /// Interface for handling payment operations with external payment providers.
    /// </summary>
    public interface IPaymentClient
    {
        /// <summary>
        /// Processes a payment using the provided token and amount.
        /// </summary>
        /// <param name="token">The payment token generated from user card details.</param>
        /// <param name="amount">The amount to be charged for the payment.</param>
        /// <param name="cancellationToken">Cancellation token if needed.</param>
        /// <returns>True if payment was successful, false otherwise.</returns>
        Task<bool> ProcessPaymentAsync(string token, float amount, CancellationToken cancellationToken = default);
    }
}
