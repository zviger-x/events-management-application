using System.Security.Claims;

namespace EventsAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <exception cref="UnauthorizedAccessException"/>
        public static Guid GetUserIdOrThrow(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User is not authorized.");

            return userId;
        }
    }
}
