using DataAccess.Entities;
using Shared.Repositories.Interfaces;

namespace DataAccess.Repositories.Interfaces
{
    public interface IUserTransactionRepository : IRepository<UserTransaction>
    {
        /// <summary>
        /// Returns a collection of user transactions by the user's id.
        /// </summary>
        /// <param name="id">User's id.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>A collection of user transactions.</returns>
        Task<IEnumerable<UserTransaction>> GetByUserIdAsync(Guid id, CancellationToken token = default);
    }
}
