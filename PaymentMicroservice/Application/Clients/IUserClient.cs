using Application.Contracts;

namespace Application.Clients
{
    /// <summary>
    /// Interface for interacting with the User microservice to manage user-related data.
    /// </summary>
    public interface IUserClient
    {
        /// <summary>
        /// Creates a new user transaction in the system.
        /// </summary>
        /// <param name="transaction">The transaction data to be saved.</param>
        /// <param name="cancellationToken">Cancellation token if needed.</param>
        /// <returns>True if transaction was successfully created, false otherwise.</returns>
        Task<bool> CreateTransactionAsync(CreateUserTransactionDto transaction, CancellationToken cancellationToken = default);
    }
}
