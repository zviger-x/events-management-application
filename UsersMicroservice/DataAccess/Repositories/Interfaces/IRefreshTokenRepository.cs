using DataAccess.Entities;

namespace DataAccess.Repositories.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        /// <summary>
        /// Returns the token by user id.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>User.</returns>
        Task<RefreshToken> GetByUserIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Updates an existing one, otherwise creates a new one
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        Task UpsertAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    }
}
