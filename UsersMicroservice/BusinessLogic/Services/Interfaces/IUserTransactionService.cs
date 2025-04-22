using BusinessLogic.Services.Interfaces.Common;
using DataAccess.Entities;

namespace BusinessLogic.Services.Interfaces
{
    public interface IUserTransactionService : ICreateService<UserTransaction>,
        IUpdateService<UserTransaction>,
        IReadService<UserTransaction>,
        IDeleteService
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
