using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Shared.Exceptions.ServerExceptions;
using System.Text;
using System.Text.Json;
using static Shared.Logging.Extensions.SerilogExtensions;
using FluentValidationException = FluentValidation.ValidationException;

namespace Shared.Common
{
    internal class ErrorHandlingHelper
    {
        public int GetResponseStatusCode(Exception ex)
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
                case Exception:
                default:
                    return StatusCodes.Status500InternalServerError;
            }
        }

        public void LogError(Exception ex, int statusCode)
        {
            if (statusCode < StatusCodes.Status500InternalServerError)
            {
                Log.ErrorInterpolated($"An error occurred: {ex.Message}");
            }
            else if (ex is RpcException rpcEx)
            {
                var errors = GetErrorObject(rpcEx);

                Log.ErrorInterpolated(ex, $"An unexpected error occurred{Environment.NewLine}Founded errors:{Environment.NewLine}{errors}");
            }
            else
            {
                Log.ErrorInterpolated(ex, $"An unexpected error occurred");
            }
        }

        public object GetErrorResponse(Exception ex)
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

        public bool TryConvertToServerException(ref Exception ex)
        {
            switch (ex)
            {
                case ArgumentNullException ane:
                    ex = new ParameterNullException(ane);
                    break;
                case ArgumentException ae:
                    ex = new ParameterException(ae);
                    break;
                default:
                    return false;
            }
            return true;
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
        protected virtual object GetErrorObject(string code, string message, string propertyName = null)
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
        protected virtual object GetErrorObject(ServerException ex)
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
        protected virtual object GetErrorObject(IEnumerable<ServerException> exceptions)
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
        protected virtual object GetErrorObject(RpcException ex)
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
                    Log.ErrorInterpolated($"Failed to parse error details: {e.Message}");
                }
            }

            return null;
        }
    }
}
