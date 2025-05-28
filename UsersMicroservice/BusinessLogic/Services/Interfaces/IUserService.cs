using BusinessLogic.Contracts;
using Shared.Common;

namespace BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Deletes the user with the specified ID.
        /// </summary>
        /// <param name="targetUserId">The ID of the user to delete.</param>
        /// <param name="currentUserId">The ID of the current user performing the action.</param>
        /// <param name="isAdmin">Indicates whether the current user has administrative privileges.</param>
        /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, CancellationToken token = default);

        /// <summary>
        /// Gets the user with the specified ID.
        /// </summary>
        /// <param name="targetUserId">The ID of the user to retrieve.</param>
        /// <param name="currentUserId">The ID of the current user performing the action.</param>
        /// <param name="isAdmin">Indicates whether the current user has administrative privileges.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The DTO representing the entity.</returns>
        Task<UserDto> GetByIdAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, CancellationToken token = default);

        /// <summary>
        /// Gets a list of all users.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>List of users.</returns>
        Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken token = default);

        /// <summary>
        /// Gets a paged collection of users.
        /// </summary>
        /// <param name="pageParameters">Page number and size.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Paged collection of users.</returns>
        Task<PagedCollection<UserDto>> GetPagedAsync(PageParameters pageParameters, CancellationToken token = default);

        /// <summary>
        /// Updates the user's profile information.
        /// </summary>
        /// <param name="targetUserId">The ID of the user to retrieve.</param>
        /// <param name="currentUserId">The ID of the current user performing the action.</param>
        /// <param name="isAdmin">Indicates whether the current user has administrative privileges.</param>
        /// <param name="userUpdate">The data to update the user's profile.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task UpdateUserProfileAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, UpdateUserDto userUpdate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <param name="targetUserId">The ID of the user to retrieve.</param>
        /// <param name="currentUserId">The ID of the current user performing the action.</param>
        /// <param name="isAdmin">Indicates whether the current user has administrative privileges.</param>
        /// <param name="changePassword">The data to change the user's password.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        Task ChangePasswordAsync(Guid targetUserId, Guid currentUserId, bool isAdmin, ChangePasswordDto changePassword, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the user's role.
        /// </summary>
        /// <param name="userRouteId">User id from route.</param>
        /// <param name="changeUserRole">The data to change the user's role.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation if needed.</param>
        /// <returns></returns>
        Task ChangeUserRoleAsync(Guid userRouteId, ChangeUserRoleDto changeUserRole, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a user with the specified ID exists
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="cancellationToken">Cancellation token if needed.</param>
        /// <returns></returns>
        Task<bool> UserExists(Guid userId, CancellationToken cancellationToken = default);
    }
}
