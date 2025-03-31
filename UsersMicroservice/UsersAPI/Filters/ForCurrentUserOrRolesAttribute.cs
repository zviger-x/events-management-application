using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccess.Entities;

namespace UsersAPI.Filters
{
    /// <summary>
    /// Determines whether the user ID passed via Route applies to the current user, and grants access to anyone with the listed roles.
    /// If there is no ID in the route, it will return BadRequest
    /// </summary>
    public class ForCurrentUserOrRolesAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRoles[] _allowedRoles;

        public ForCurrentUserOrRolesAttribute(params UserRoles[] allowedRoles)
        {
            _allowedRoles = allowedRoles ?? Array.Empty<UserRoles>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => Enum.Parse<UserRoles>(c.Value)).ToArray();

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            var routeId = context.RouteData.Values["id"]?.ToString();
            if (routeId == null || !Guid.TryParse(routeId, out var requestedId))
            {
                context.Result = new BadRequestResult();
                return;
            }

            // Запрещаем, если пользователь запрашивает НЕ себя или если у него нет одной из разрешённых ролей
            if (userId != requestedId && !_allowedRoles.Any(role => userRoles.Contains(role)))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
