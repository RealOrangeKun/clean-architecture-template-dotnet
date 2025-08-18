using FluentResults;

namespace Project.Common.Domain;

public static class ErrorMetadata
{
    public const string ErrorTypeKey = "ErrorType";
    public const string ValidationFailuresKey = "ValidationFailures";

    public static Error WithErrorType(this Error error, ErrorType errorType)
    {
        return error.WithMetadata(ErrorTypeKey, errorType);
    }

    public static bool TryGetErrorType(this Error error, out ErrorType type)
    {
        if (error.Metadata.TryGetValue(ErrorTypeKey, out object? value) &&
            value is ErrorType enumValue)
        {
            type = enumValue;
            return true;
        }
        type = default;
        return false;
    }

    public static ErrorType? GetErrorType(this Error error)
    {
        return error.TryGetErrorType(out ErrorType type)
            ? type
            : null;
    }

}
