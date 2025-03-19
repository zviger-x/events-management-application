using DataAccess.Entities;

namespace DataAccess.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Checks if an email is in a collection
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <returns>Returns true if contained</returns>
        Task<bool> ContainsEmailAsync(string email, CancellationToken token = default);

        /// <summary>
        /// Returns the user by his email.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <returns>User.</returns>
        Task<User> GetByEmailAsync(string email, CancellationToken token = default);
    }
}
