using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetLib.AspNetCore.Handlers
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        public CustomExceptionHandler()
        {
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            switch (exception)
            {
                case UnauthorizedAccessException _:
                    await HandleUnauthorizedAccessException(httpContext, exception);
                    return true;
                case ArgumentException _:
                    await HandleApplicationException(httpContext, exception);
                    return true;
                case ApplicationException _:
                    await HandleApplicationException(httpContext, exception);
                    return true;
                default:
                    await HandleException(httpContext, exception);
                    return true;
            }
            return false;
        }

        private async Task HandleException(HttpContext httpContext, Exception ex)
        {
            var exception = (Exception)ex;

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Internal Server Error.",
                Detail = exception.Message
            });
        }

        private async Task HandleApplicationException(HttpContext httpContext, Exception ex)
        {
            var exception = (ApplicationException)ex;

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Invalid request.",
                Detail = exception.Message
            });
        }

        private async Task HandleUnauthorizedAccessException(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });
        }

    }
}
