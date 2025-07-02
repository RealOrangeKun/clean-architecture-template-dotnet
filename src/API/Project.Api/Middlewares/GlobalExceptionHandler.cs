using System.Text.Json;
using Project.Common.Presentation.Results;
using Microsoft.AspNetCore.Diagnostics;

namespace Project.Api.Middlewares;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhandled exception occurred. {Message}", exception.Message);

        httpContext.Response.ContentType = "application/json";

        if (exception is BadHttpRequestException ||
            exception is JsonException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            var errorResponse = ApiResponse.Create(
                success: false,
                message: "Invalid or missing request body.",
                errors: ["The request body could not be processed."]);
            await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var genericError = ApiResponse.Create(
            success: false,
            message: "An unexpected error occurred. Please try again later.",
            errors: ["Internal server error."]);
        await httpContext.Response.WriteAsJsonAsync(genericError, cancellationToken);

        return true;
    }
}
