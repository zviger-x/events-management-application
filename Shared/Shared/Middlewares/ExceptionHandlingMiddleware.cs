using Microsoft.AspNetCore.Http;
using Shared.Common;

namespace Shared.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ErrorHandlingHelper _errorHandlingHelper;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
            _errorHandlingHelper = new ErrorHandlingHelper();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _errorHandlingHelper.TryConvertToServerException(ref ex);

                var responseStatus = _errorHandlingHelper.GetResponseStatusCode(ex);
                var response = _errorHandlingHelper.GetErrorResponse(ex);

                _errorHandlingHelper.LogError(ex, responseStatus);

                await SendErrorAsJsonAsync(context, response, responseStatus);
            }
        }

        private async Task SendErrorAsJsonAsync(HttpContext context, object response, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
