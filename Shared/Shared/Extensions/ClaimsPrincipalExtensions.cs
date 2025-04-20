using Shared.Enums;
using Shared.Exceptions.ServerExceptions;
using System.Security.Claims;

namespace Shared.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Retrieves the current user's ID or throws an exception if the user is not authenticated.
        /// </summary>
        /// <returns>The user's ID.</returns>
        /// <exception cref="UnauthorizedException">Thrown if the user is not authenticated.</exception>
        public static Guid GetUserIdOrThrow(this ClaimsPrincipal user)
        {
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User is not authorized.");

            return userId;
        }

        /// <summary>
        /// Checks if the current user has the specified role, throws an exception if not.
        /// </summary>
        /// <param name="role">The role to check.</param>
        /// <returns>True if the user has the specified role, otherwise false.</returns>
        /// <exception cref="UnauthorizedException">Thrown if the user is not authenticated.</exception>

        public static bool IsInRoleOrThrow(this ClaimsPrincipal user, UserRoles role)
        {
            var hasRole = user?.IsInRole(role.ToString());

            if (hasRole == null)
                throw new UnauthorizedException("You are not authorized to perform this action.");

            return hasRole.Value;
        }

        /// <summary>
        /// Determines if the current user is an admin. Throws an exception if the user is not authenticated.
        /// </summary>
        /// <returns>True if the user is an admin, otherwise false.</returns>
        /// <exception cref="UnauthorizedException">Thrown if the user is not authenticated.</exception>

        public static bool IsAdminOrThrow(this ClaimsPrincipal user)
        {
            return IsInRoleOrThrow(user, UserRoles.Admin);
        }
    }
}
