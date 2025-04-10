using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Exceptions.ServerExceptions;
using FluentValidationException = FluentValidation.ValidationException;

namespace Shared.Middlewares
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
            catch (Exception ex)
            {
                var responseStatus = GetResponseStatusCode(ex);
                var response = ex switch
                {
                    // Ошибки в валидаторах
                    FluentValidationException fluentValidationEx =>
                        GetErrorResponse(ValidationException.GetValidationExceptions(fluentValidationEx)),

                    // Ошибки в бизнес логике
                    ServerException serverEx => GetErrorResponse(serverEx),

                    // Любые другие ошибки
                    _ => GetErrorResponse("unexpectedError", "An unexpected error occurred")
                };

                LogError(ex, responseStatus);

                await SendErrorAsJsonAsync(context, response, responseStatus);
            }
        }

        private async Task SendErrorAsJsonAsync(HttpContext context, object response, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }

        private int GetResponseStatusCode(Exception ex)
        {
            switch (ex)
            {
                case FluentValidationException:
                case ValidationException:
                case ParameterException:
                    return StatusCodes.Status400BadRequest;

                case NotFoundException:
                    return StatusCodes.Status404NotFound;

                case UnauthorizedException:
                    return StatusCodes.Status401Unauthorized;

                case ForbiddenAccessException:
                    return StatusCodes.Status403Forbidden;

                case ConflictException:
                    return StatusCodes.Status409Conflict;

                case ServerException:
                case Exception:
                default:
                    return StatusCodes.Status500InternalServerError;
            }
        }

        private void LogError(Exception ex, int statusCode)
        {
            if (statusCode < StatusCodes.Status500InternalServerError)
                _logger.LogError($"An error occurred: {ex.Message}");
            else
                _logger.LogError(ex, "An unexpected error occurred");
        }

        /// <summary>
        /// Returns an object in a standardized format:
        /// <code>
        /// {
        ///     "errors": {
        ///         "unexpectedError": {
        ///             "propertyName": null,
        ///             "serverMessage": "An unexpected error occurred."
        ///         }
        ///     }
        /// }
        /// </code>
        /// </summary>
        private object GetErrorResponse(string code, string message, string propertyName = null)
        {
            return GetErrorResponse(new ServerException(code, message, propertyName));
        }

        /// <summary>
        /// Returns an object in a standardized format:
        /// <code>
        /// {
        ///     "errors": {
        ///         "unexpectedError": {
        ///             "propertyName": null,
        ///             "serverMessage": "An unexpected error occurred."
        ///         }
        ///     }
        /// }
        /// </code>
        /// </summary>
        private object GetErrorResponse(ServerException ex)
        {
            return GetErrorResponse([ex]);
        }

        /// <summary>
        /// Returns an object in a standardized format:
        /// <code>
        /// {
        ///     "errors": {
        ///         "unexpectedError": {
        ///             "propertyName": null,
        ///             "serverMessage": "An unexpected error occurred."
        ///         }
        ///     }
        /// }
        /// </code>
        /// </summary>
        private object GetErrorResponse(IEnumerable<ServerException> exceptions)
        {
            var errorDictionary = exceptions.ToDictionary(
                ex => ex.ErrorCode,
                ex => new { ex.PropertyName, ex.Message });

            return new { errors = errorDictionary };
        }
    }
}
