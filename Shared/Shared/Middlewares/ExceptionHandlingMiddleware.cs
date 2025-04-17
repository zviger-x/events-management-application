using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Exceptions.ServerExceptions;
using Shared.Logging.Extensions;
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
                TryConvertToServerException(ref ex);

                var responseStatus = GetResponseStatusCode(ex);
                var response = GetErrorResponse(ex);

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

        private object GetErrorResponse(Exception ex)
        {
            return ex switch
            {
                // Ошибки в валидаторах
                FluentValidationException fluentValidationEx =>
                    GetErrorObject(ValidationException.GetValidationExceptions(fluentValidationEx)),

                // Ошибки в бизнес логике
                ServerException serverEx => GetErrorObject(serverEx),

                // Любые другие ошибки
                _ => GetErrorObject("unexpectedError", "An unexpected error occurred")
            };
        }

        private bool TryConvertToServerException(ref Exception ex)
        {
            ex = ex switch
            {
                ArgumentNullException argNullEx => new ParameterNullException(argNullEx),
                ArgumentException argEx => new ParameterException(argEx),
                _ => ex
            };

            return ex is ServerException;
        }

        private void LogError(Exception ex, int statusCode)
        {
            if (statusCode < StatusCodes.Status500InternalServerError)
                _logger.LogErrorInterpolated($"An error occurred: {ex.Message}");
            else
                _logger.LogErrorInterpolated(ex, $"An unexpected error occurred");
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
        private object GetErrorObject(string code, string message, string propertyName = null)
        {
            return GetErrorObject(new ServerException(code, message, propertyName));
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
        private object GetErrorObject(ServerException ex)
        {
            return GetErrorObject([ex]);
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
        private object GetErrorObject(IEnumerable<ServerException> exceptions)
        {
            var errorDictionary = exceptions.ToDictionary(
                ex => ex.ErrorCode,
                ex => new { ex.PropertyName, ex.Message });

            return new { errors = errorDictionary };
        }
    }
}
