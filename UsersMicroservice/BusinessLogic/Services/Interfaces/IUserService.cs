using BusinessLogic.Contracts;
using DataAccess.Entities;

namespace BusinessLogic.Services.Interfaces
{
    public interface IUserService : IService<User>
    {
        /// <summary>
        /// Updates the user's profile information.
        /// </summary>
        /// <param name="userUpdate">The data to update the user's profile.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task UpdateUserProfileAsync(UpdateUserDTO userUpdate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <param name="changePassword">The data to change the user's password.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task ChangePasswordAsync(ChangePasswordDTO changePassword, CancellationToken cancellationToken = default);
    }
}
