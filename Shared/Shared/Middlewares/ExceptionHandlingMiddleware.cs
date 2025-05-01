using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Exceptions.ServerExceptions;
using Shared.Logging.Extensions;
using System.Text;
using System.Text.Json;
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
                case PaymentException:
                    return StatusCodes.Status409Conflict;

                case ServerException:
                case RpcException:
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

                // Ошибки в gRPC запросах
                RpcException rpcEx => GetErrorObject(rpcEx),

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
            {
                _logger.LogErrorInterpolated($"An error occurred: {ex.Message}");
            }
            else if (ex is RpcException rpcEx)
            {
                var errors = GetErrorObject(rpcEx);

                _logger.LogErrorInterpolated(ex, $"An unexpected error occurred{Environment.NewLine}Founded errors:{Environment.NewLine}{errors}");
            }
            else
            {
                _logger.LogErrorInterpolated(ex, $"An unexpected error occurred");
            }
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
        /// If the object cannot be parsed, returns null.
        /// </summary>
        private object GetErrorObject(RpcException ex)
        {
            var trailer = ex.Trailers.Get("error-details-bin");
            if (trailer != null)
            {
                try
                {
                    var bytes = trailer.ValueBytes;
                    var json = Encoding.UTF8.GetString(bytes);
                    var errorObject = JsonSerializer.Deserialize<object>(json);

                    return errorObject;
                }
                catch (Exception e)
                {
                    _logger.LogErrorInterpolated($"Failed to parse error details: {e.Message}");
                }
            }

            return null;
        }
    }
}
