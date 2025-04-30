using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Exceptions.ServerExceptions;
using Shared.Logging.Extensions;
using System.Text.Json;
using FluentValidationException = FluentValidation.ValidationException;

namespace Shared.Grpc.Interceptors
{
    public class GrpcExceptionInterceptor : Interceptor
    {
        private readonly ILogger<GrpcExceptionInterceptor> _logger;

        public GrpcExceptionInterceptor(ILogger<GrpcExceptionInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                TryConvertToServerException(ref ex);

                var httpStatus = GetResponseStatusCode(ex);
                var errorBody = GetErrorResponse(ex);

                LogError(ex, httpStatus);

                var grpcStatus = MapToGrpcStatusCode(httpStatus);

                throw GetRpcException(grpcStatus, ex.Message, errorBody);
            }
        }

        private RpcException GetRpcException(StatusCode grpcStatus, string message, object errors)
        {
            var status = new Status(grpcStatus, message);
            var errorsAsBytes = JsonSerializer.SerializeToUtf8Bytes(errors);
            var trailers = new Metadata { { new Metadata.Entry("error-details-bin", errorsAsBytes) } };

            return new RpcException(status, trailers);
        }

        private StatusCode MapToGrpcStatusCode(int httpStatus)
        {
            return httpStatus switch
            {
                StatusCodes.Status400BadRequest => StatusCode.InvalidArgument,
                StatusCodes.Status401Unauthorized => StatusCode.Unauthenticated,
                StatusCodes.Status403Forbidden => StatusCode.PermissionDenied,
                StatusCodes.Status404NotFound => StatusCode.NotFound,
                StatusCodes.Status409Conflict => StatusCode.Aborted,
                >= StatusCodes.Status500InternalServerError => StatusCode.Internal,
                _ => StatusCode.Unknown
            };
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
                case Exception:
                default:
                    return StatusCodes.Status500InternalServerError;
            }
        }

        private void LogError(Exception ex, int statusCode)
        {
            if (statusCode < StatusCodes.Status500InternalServerError)
                _logger.LogErrorInterpolated($"An error occurred: {ex.Message}");
            else
                _logger.LogErrorInterpolated(ex, $"An unexpected error occurred");
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
