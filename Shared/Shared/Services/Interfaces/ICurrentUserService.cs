using Shared.Enums;
using Shared.Exceptions.ServerExceptions;

namespace Shared.Services.Interfaces
{
    public interface ICurrentUserService
    {
        /// <summary>
        /// Retrieves the current user's ID or throws an exception if the user is not authenticated.
        /// </summary>
        /// <returns>The user's ID.</returns>
        /// <exception cref="UnauthorizedException">Thrown if the user is not authenticated.</exception>
        Guid GetUserIdOrThrow();

        /// <summary>
        /// Checks if the current user has the specified role, throws an exception if not.
        /// </summary>
        /// <param name="role">The role to check.</param>
        /// <returns>True if the user has the specified role, otherwise false.</returns>
        /// <exception cref="UnauthorizedException">Thrown if the user is not authenticated.</exception>
        bool IsInRoleOrThrow(UserRoles role);

        /// <summary>
        /// Determines if the current user is an admin. Throws an exception if the user is not authenticated.
        /// </summary>
        /// <returns>True if the user is an admin, otherwise false.</returns>
        /// <exception cref="UnauthorizedException">Thrown if the user is not authenticated.</exception>
        bool IsAdminOrThrow();
    }
}
