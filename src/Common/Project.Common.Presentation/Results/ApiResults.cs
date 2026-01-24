using FluentResults;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Project.Common.Domain;

namespace Project.Common.Presentation.Results;

public static class ApiResults
{
    public static IResult Problem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create a problem result from a successful result.");
        }

        IError firstError = result.Errors.Count > 0 ? result.Errors[0] : new Error("Unknown error");
        if (firstError is not Error error)
        {
            error = new Error("Unknown error");
        }

        ErrorType errorType = error.TryGetErrorType(out ErrorType type) ? type : ErrorType.Failure;

        return Microsoft.AspNetCore.Http.Results.Problem(
            title: GetTitle(errorType),
            detail: GetDetail(error, errorType),
            type: GetType(errorType),
            statusCode: GetStatusCode(errorType),
            extensions: GetErrors(result));

        static string GetTitle(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => "Validation Error",
                ErrorType.Problem => "Bad Request",
                ErrorType.NotFound => "Not Found",
                ErrorType.Conflict => "Conflict",
                ErrorType.Unauthorized => "Unauthorized",
                ErrorType.Forbidden => "Forbidden",
                ErrorType.TooManyRequests => "Too Many Requests",
                _ => "Server Error"
            };

        static string GetDetail(Error error, ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => "One or more validation errors occurred.",
                ErrorType.Problem => error.Message,
                ErrorType.NotFound => error.Message,
                ErrorType.Conflict => error.Message,
                ErrorType.Unauthorized => error.Message,
                ErrorType.Forbidden => error.Message,
                ErrorType.TooManyRequests => error.Message,
                _ => "An unexpected error occurred."
            };

        static string GetType(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                ErrorType.TooManyRequests => "https://tools.ietf.org/html/rfc6585#section-4",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

        static int GetStatusCode(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Problem => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.TooManyRequests => StatusCodes.Status429TooManyRequests,
                _ => StatusCodes.Status500InternalServerError
            };

        static Dictionary<string, object?>? GetErrors(Result result)
        {
            if (result.Errors.Count == 0)
            {
                return null;
            }

            IError firstError = result.Errors[0];
            if (firstError is Error error && error.Metadata.TryGetValue(ErrorMetadata.ValidationFailuresKey, out object? failuresObj) &&
                failuresObj is ValidationFailure[] failures)
            {
                var validationErrors = failures
                    .GroupBy(f => f.PropertyName)
                    .ToDictionary(
                        g => g.Key.ToLowerInvariant(),
                        g => g.Select(f => f.ErrorMessage).ToArray() as object
                    );

                return new Dictionary<string, object?>
                {
                    { "errors", validationErrors }
                };
            }

            return null;
        }
    }

    public static IResult Problem<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create a problem result from a successful result.");
        }

        // Convert to base Result and call the main Problem method
        Result baseResult = Result.Fail([.. result.Errors]);
        return Problem(baseResult);
    }
}