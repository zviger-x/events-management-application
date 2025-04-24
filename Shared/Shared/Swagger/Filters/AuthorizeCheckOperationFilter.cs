using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.Swagger.Filters
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodInfo = context.MethodInfo;

            var hasAuthorize = methodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                methodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            var allowAnonymous = methodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

            if (!hasAuthorize || allowAnonymous)
                return;

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                }
            };
        }

        private bool IsAuthorizeAttribute(object attribute)
        {
            return attribute is AuthorizeAttribute || attribute.GetType().IsSubclassOf(typeof(AuthorizeAttribute));
        }
    }
}
