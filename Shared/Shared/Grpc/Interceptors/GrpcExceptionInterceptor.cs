using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using Shared.Common;
using System.Text.Json;

namespace Shared.Grpc.Interceptors
{
    public class GrpcExceptionInterceptor : Interceptor
    {
        private readonly ErrorHandlingHelper _errorHandlingHelper;

        public GrpcExceptionInterceptor()
        {
            _errorHandlingHelper = new ErrorHandlingHelper();
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                _errorHandlingHelper.TryConvertToServerException(ref ex);

                var statusCode = _errorHandlingHelper.GetResponseStatusCode(ex);
                var errorBody = _errorHandlingHelper.GetErrorResponse(ex);

                _errorHandlingHelper.LogError(ex, statusCode);

                var grpcStatus = MapToGrpcStatusCode(statusCode);

                throw GetRpcException(grpcStatus, ex.Message, errorBody);
            }
        }

        private RpcException GetRpcException(StatusCode grpcStatus, string message, object errors)
        {
            var status = new Status(grpcStatus, message);
            var errorsAsBytes = JsonSerializer.SerializeToUtf8Bytes(errors);
            var trailers = new Metadata
            {
                { new Metadata.Entry("error-details-bin", errorsAsBytes) },
                { new Metadata.Entry("grpc-status-details-bin", errorsAsBytes) }
            };

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
    }
}
