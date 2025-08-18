using FluentResults;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Project.Common.Domain;
using System.Reflection;

namespace Project.Common.Presentation.Results;

public static class ApiResult
{
    public static IResult ToApiResult<T>(this Result<T> result)
    {
        int statusCode = GetStatusCode(result);
        var apiResponse = result.ToApiResponse();
        IEnumerable<string>? validationErrors = ExtractValidationErrors(result);
        if (validationErrors is not null)
        {
            Type responseType = typeof(ApiResponse<T>);
            PropertyInfo errorsProp = responseType.GetProperty(nameof(ApiResponse<T>.Errors))!;
            PropertyInfo messageProp = responseType.GetProperty(nameof(ApiResponse<T>.Message))!;

            errorsProp?.SetValue(apiResponse, validationErrors);
            messageProp?.SetValue(apiResponse, "One or More Validation errors occurred");
        }
        return Microsoft.AspNetCore.Http.Results.Json(apiResponse, statusCode: statusCode);
    }
    public static IResult ToApiResult(this Result result)
    {
        int statusCode = GetStatusCode(result);
        var apiResponse = result.ToApiResponse();
        IEnumerable<string>? validationErrors = ExtractValidationErrors(result);
        if (validationErrors is not null)
        {
            Type responseType = typeof(ApiResponse);
            PropertyInfo errorsProp = responseType.GetProperty(nameof(ApiResponse.Errors))!;
            PropertyInfo messageProp = responseType.GetProperty(nameof(ApiResponse.Message))!;

            errorsProp?.SetValue(apiResponse, validationErrors);
            messageProp?.SetValue(apiResponse, "One or More Validation errors occurred");

        }
        return Microsoft.AspNetCore.Http.Results.Json(apiResponse, statusCode: statusCode);
    }

    private static IEnumerable<string>? ExtractValidationErrors(ResultBase result)
    {
        if (result.Errors.Count > 0 && result.Errors[0] is Error error &&
            error.Metadata.TryGetValue(ErrorMetadata.ValidationFailuresKey, out object? failuresObj) &&
            failuresObj is ValidationFailure[] failuresArr)
        {
            return failuresArr.Select(f => $"{f.PropertyName}: {f.ErrorMessage}");
        }
        return null;
    }

    private static int GetStatusCode(Result result)
    {
        if (result.IsSuccess)
        {
            ISuccess? success = result.Successes.Count > 0 ? result.Successes[0] : null;
            if (success is Success s)
            {
                if (s.Metadata.TryGetValue(SuccessMetadata.StatusCodeKey, out object? statusCodeObj) && statusCodeObj is int statusCode)
                {
                    return statusCode;
                }
            }
        }

        IError? firstError = result.Errors.Count > 0 ? result.Errors[0] : null;

        if (firstError is Error e)
        {
            if (e.TryGetErrorType(out ErrorType errorType))
            {
                return errorType switch
                {
                    ErrorType.Validation => StatusCodes.Status400BadRequest,
                    ErrorType.Conflict => StatusCodes.Status409Conflict,
                    ErrorType.NotFound => StatusCodes.Status404NotFound,
                    ErrorType.Problem => StatusCodes.Status400BadRequest,
                    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };
            }
        }

        return StatusCodes.Status500InternalServerError;
    }

    private static int GetStatusCode<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            ISuccess? success = result.Successes.Count > 0 ? result.Successes[0] : null;
            if (success is Success s)
            {
                if (s.Metadata.TryGetValue(SuccessMetadata.StatusCodeKey, out object? statusCodeObj) && statusCodeObj is int statusCode)
                {
                    return statusCode;
                }
            }
        }

        IError? firstError = result.Errors.Count > 0 ? result.Errors[0] : null;

        if (firstError is Error e)
        {
            if (e.TryGetErrorType(out ErrorType errorType))
            {
                return errorType switch
                {
                    ErrorType.Validation => StatusCodes.Status400BadRequest,
                    ErrorType.Conflict => StatusCodes.Status409Conflict,
                    ErrorType.NotFound => StatusCodes.Status404NotFound,
                    ErrorType.Problem => StatusCodes.Status400BadRequest,
                    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };
            }
        }

        return StatusCodes.Status500InternalServerError;
    }
}
