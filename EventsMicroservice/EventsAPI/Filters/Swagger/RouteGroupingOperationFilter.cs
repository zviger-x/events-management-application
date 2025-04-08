using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EventsAPI.Filters.Swagger
{
    public class RouteGroupingOperationFilter : IOperationFilter
    {
        private readonly int _groupByWordIndex;

        public RouteGroupingOperationFilter(int groupByWordIndex)
        {
            _groupByWordIndex = groupByWordIndex;
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
