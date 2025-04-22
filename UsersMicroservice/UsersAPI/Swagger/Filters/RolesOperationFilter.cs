using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UsersAPI.Swagger.Filters
{
    public class RolesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeAttributes = context.MethodInfo
                .DeclaringType?.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>();

            var allowAnonymous = context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

            if (allowAnonymous || authorizeAttributes == null || !authorizeAttributes.Any())
                return;

            var roles = authorizeAttributes
                .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
                .SelectMany(a => a.Roles.Split(','))
                .Select(r => r.Trim())
                .Distinct();

            var description = "Authorization required";
            if (roles.Any())
                description += $"<br/><br/>Required roles: <b>{string.Join(", ", roles)}</b>";

            operation.Description = string.IsNullOrEmpty(operation.Description)
                ? description
                : $"{operation.Description}<br/><br/>{description}";
        }
    }
}
