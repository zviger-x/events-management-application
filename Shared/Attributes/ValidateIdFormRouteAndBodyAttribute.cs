using Microsoft.AspNetCore.Mvc.Filters;

namespace Shared.Attributes
{
    public class ValidateIdFormRouteAndBodyAttribute : ActionFilterAttribute
    {
        private readonly string _routeParamName;
        private readonly string _bodyParamName;
        private readonly string _modelPropertyName;

        public ValidateIdFormRouteAndBodyAttribute(string routeParamName, string bodyParamName, string modelPropertyName)
        {
            _routeParamName = routeParamName;
            _bodyParamName = bodyParamName;
            _modelPropertyName = modelPropertyName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Получаем параметр Id (_routeParamName) из роута
            if (!context.RouteData.Values.TryGetValue(_routeParamName, out var routeValue) || routeValue == null)
                throw new ArgumentException($"Required parameter \"{_routeParamName}\" not provided in the route.");

            // Парсим Id (_routeParamName) из роута в Guid
            if (!Guid.TryParse(routeValue.ToString(), out Guid routeId))
                throw new ArgumentException($"Invalid value for route parameter \"{_routeParamName}\".");

            // Получаем [FromBody] модель по десериализованному полю (_bodyParamName)
            if (!context.ActionArguments.TryGetValue(_bodyParamName, out var bodyValue) || bodyValue == null)
                throw new ArgumentException($"Failed to retrieve data for \"{_bodyParamName}\" from the request body.");

            // Получаем поле Id (_modelPropertyName) непосредственно в модели
            var property = bodyValue.GetType().GetProperty(_modelPropertyName);
            if (property == null)
                throw new ArgumentException($"Field \"{_modelPropertyName}\" is missing in the provided data.");

            // Получаем значение этого поля Id (_modelPropertyName)
            var modelValue = property.GetValue(bodyValue);
            if (modelValue == null)
                throw new ArgumentException($"Missing value for field \"{_modelPropertyName}\".");

            // Парсим значение полученного поля Id (_modelPropertyName)
            if (!(modelValue is Guid modelId))
                throw new ArgumentException($"Field \"{_modelPropertyName}\" contains an invalid identifier format.");

            // Сравниваем Id в роуте и в модели
            if (routeId != modelId)
                throw new ArgumentException("You do not have access to the object with the specified ID. Please verify the provided data and try again.");

            base.OnActionExecuting(context);
        }
    }
}
