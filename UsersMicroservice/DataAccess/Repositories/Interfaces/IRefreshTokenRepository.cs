using DataAccess.Entities;

namespace DataAccess.Repositories.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        /// <summary>
        /// Returns the token by user id.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>User.</returns>
        Task<RefreshToken> GetByUserIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Returns the full refresh token data, from the refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token.</param>
        /// <param name="token">Cancellation token to cancel the operation if needed.</param>
        /// <returns>Full refresh token data.</returns>
        Task<RefreshToken> GetByRefreshTokenAsync(string refreshToken, CancellationToken token = default);

        /// <summary>
        /// Updates an existing one, otherwise creates a new one
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task UpsertAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    }
}
