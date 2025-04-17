using BusinessLogic.Contracts;
using DataAccess.Entities;

namespace BusinessLogic.Services.Interfaces
{
    public interface IUserService : IService<User>
    {
        /// <summary>
        /// Updates the user's profile information.
        /// </summary>
        /// <param name="userRouteId">User id from route.</param>
        /// <param name="userUpdate">The data to update the user's profile.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task UpdateUserProfileAsync(Guid userRouteId, UpdateUserDTO userUpdate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <param name="userRouteId">User id from route.</param>
        /// <param name="changePassword">The data to change the user's password.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task ChangePasswordAsync(Guid userRouteId, ChangePasswordDTO changePassword, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the user's role.
        /// </summary>
        /// <param name="userRouteId">User id from route.</param>
        /// <param name="changeUserRole">The data to change the user's role.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns></returns>
        Task ChangeUserRoleAsync(Guid userRouteId, ChangeUserRoleDTO changeUserRole, CancellationToken cancellationToken = default);
    }
}
