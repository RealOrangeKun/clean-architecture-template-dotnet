using Microsoft.AspNetCore.Diagnostics;
using Project.Common.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Project.Api.Middlewares;

internal sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhandled exception occurred. {Message}", exception.Message);

        httpContext.Response.ContentType = "application/json";

        httpContext.Response.StatusCode = exception switch
        {
            ProjectException => StatusCodes.Status400BadRequest,
            _ => 500
        };



        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = "An error occurred while processing your request.",
                Detail = exception.Message
            }
        });
    }
}
