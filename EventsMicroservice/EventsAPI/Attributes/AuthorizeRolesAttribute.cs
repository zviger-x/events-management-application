using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace EventsAPI.Attributes
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params UserRoles[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}
