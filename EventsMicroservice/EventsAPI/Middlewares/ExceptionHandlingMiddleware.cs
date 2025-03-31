using Application.Exceptions;
using FluentValidation;

namespace EventsAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex.Message, "Validation exception occurred");
                var response = new
                {
                    errors = ex.Errors.ToDictionary(
                        e => e.ErrorCode,
                        e => new { e.PropertyName, serverMessage = e.ErrorMessage })
                };

                await SendErrorAsJsonAsync(context, response, StatusCodes.Status400BadRequest);
            }
            catch (ServiceValidationException ex)
            {
                _logger.LogError(ex, "Service validation exception occurred");
                var response = GetSingleErrorResponse(ex.ErrorCode, ex.Message, ex.PropertyName);

                await SendErrorAsJsonAsync(context, response, StatusCodes.Status400BadRequest);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception occurred");
                var response = GetSingleErrorResponse("invalidArgument", ex.Message, ex.ParamName!);

                await SendErrorAsJsonAsync(context, response, StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                var response = GetSingleErrorResponse("unexpectedError", "An unexpected error occurred");

                await SendErrorAsJsonAsync(context, response, StatusCodes.Status500InternalServerError);
            }
        }

        private async Task SendErrorAsJsonAsync(HttpContext context, object response, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }

        private object GetSingleErrorResponse(string code, string message, string propertyName = null!)
        {
            // {
            //     "errors": {
            //         "unexpectedError": {
            //             "propertyName": null,
            //             "serverMessage": "An unexpected error occurred."
            //         }
            //     }
            // }

            return new
            {
                errors = new Dictionary<string, object>
                {
                    [code] = new
                    {
                        propertyName = propertyName,
                        serverMessage = message
                    }
                }
            };
        }
    }
}
