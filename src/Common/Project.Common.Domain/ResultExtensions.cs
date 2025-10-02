
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Project.Common.Domain;

[Obsolete("Use Result.Ok() or Result<T>.Ok() with appropriate success messages instead")]
public static class ResultExtensions
{
    public static Result WithCustomSuccess(
        this Result result,
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
    public static Result<T> WithCustomSuccess<T>(
        this Result<T> result,
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
