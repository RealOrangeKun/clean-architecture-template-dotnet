
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Project.Common.Domain;

[Obsolete("Use Result.Ok() or Result<T>.Ok() with appropriate success messages instead")]
public static class ResultExtensions
{
    extension(Result result)
    {
        public Result WithCustomSuccess(
            string message,
            int statusCode = StatusCodes.Status200OK)
        {
            if (result.IsFailed)
            {
                return result;
            }

            Success success = new Success(message)
                .WithStatusCode(statusCode);

            return result.WithSuccess(success);
        }
    }

    extension<T>(Result<T> result)
    {
        public Result<T> WithCustomSuccess(
            string message,
            int statusCode = StatusCodes.Status200OK)
        {
            if (result.IsFailed)
            {
                return result;
            }

            Success success = new Success(message)
                .WithStatusCode(statusCode);

            return result.WithSuccess(success);
        }
    }
}
