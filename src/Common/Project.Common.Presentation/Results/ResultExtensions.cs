using FluentResults;

namespace Project.Common.Presentation.Results;

public static class ResultExtensions
{
    public static ApiResponse<T> ToApiResponse<T>(this Result<T> result)
    {
        string message = "Operation succeeded";
        if (result.IsSuccess)
        {
            message = result.Reasons.FirstOrDefault(r => r is Success)?.Message ?? message;
            return ApiResponse<T>.Create(true, message, result.Value, null);
        }
        message = "Operation failed";
        return ApiResponse<T>.Create(false, message, default, result.Errors.Select(e => e.Message));
    }

    public static ApiResponse ToApiResponse(this Result result)
    {
        string message = "Operation succeeded";
        if (result.IsSuccess)
        {
            message = result.Reasons.FirstOrDefault(r => r is Success)?.Message ?? message;
            return ApiResponse.Create(true, message, null);
        }
        message = "Operation failed";
        return ApiResponse.Create(false, message, result.Errors.Select(e => e.Message));
    }
}
