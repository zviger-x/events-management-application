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
    }
}
