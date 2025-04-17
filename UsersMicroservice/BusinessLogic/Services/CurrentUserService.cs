using BusinessLogic.Exceptions;
using BusinessLogic.Services.Interfaces;
using DataAccess.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Provides access to the current user context from Claims
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetUserIdOrThrow()
        {
            var userIdClaim = GetUser()?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User is not authorized.");

            return userId;
        }

        public bool IsInRoleOrThrow(UserRoles role)
        {
            var hasRole = _httpContextAccessor.HttpContext?.User?.IsInRole(role.ToString());

            if (hasRole == null)
                throw new UnauthorizedException("You are not authorized to perform this action.");

            return hasRole.Value;
        }

        public bool IsAdminOrThrow()
        {
            return IsInRoleOrThrow(UserRoles.Admin);
        }

        private ClaimsPrincipal GetUser()
        {
            return _httpContextAccessor.HttpContext?.User;
        }
    }
}
