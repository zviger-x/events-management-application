using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UsersAPI.Swagger.Filters
{
    public class RouteGroupingOperationFilter : IOperationFilter
    {
        private readonly int _groupByWordIndex;

        public RouteGroupingOperationFilter(int groupByWordIndex)
        {
            _groupByWordIndex = Math.Max(0, groupByWordIndex);
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var route = context.ApiDescription.RelativePath;

            if (route == null)
                return;

            var routeParts = route.Split('/');

            // Проверяем, что индекс существует в пути
            if (routeParts.Length <= _groupByWordIndex)
                return;

            var groupName = $"Routes \"{routeParts[_groupByWordIndex]}\"";
            operation.Tags = new List<OpenApiTag> { new OpenApiTag { Name = groupName } };
        }
    }
}
