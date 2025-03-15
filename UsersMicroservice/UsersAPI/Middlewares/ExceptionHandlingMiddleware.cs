using FluentValidation;

namespace UsersAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                var response = new
                {
                    errors = ex.Errors.ToDictionary(
                        e => e.ErrorCode,
                        e => new { e.PropertyName, serverMessage = e.ErrorMessage })
                };

                await SendErrorAsJsonAsync(context, response, StatusCodes.Status400BadRequest);
            }
            catch (ArgumentException ex)
            {
                var response = new { error = ex.Message };

                await SendErrorAsJsonAsync(context, response, StatusCodes.Status400BadRequest);
            }
            // catch (Exception)
            // {
            //     context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            //     await context.Response.WriteAsync("An unexpected error occurred.");
            // }
        }

        private async Task SendErrorAsJsonAsync(HttpContext context, object response, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
