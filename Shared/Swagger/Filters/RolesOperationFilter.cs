using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.Swagger.Filters
{
    public class RolesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeAttributes = context.MethodInfo
                .DeclaringType?.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>();

            if (authorizeAttributes == null || !authorizeAttributes.Any())
                return;

            var roles = authorizeAttributes
                .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
                .SelectMany(a => a.Roles.Split(','))
                .Select(r => r.Trim())
                .Distinct();

            operation.Description = $"Authorization required";

            if (roles.Any())
                operation.Description += $"<br/><br/>Required roles: <b>{string.Join(", ", roles)}<b/>";
        }
    }
}
