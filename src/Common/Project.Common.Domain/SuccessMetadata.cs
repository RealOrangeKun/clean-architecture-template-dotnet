
using FluentResults;

namespace Project.Common.Domain;

public static class SuccessMetadata
{
    public const string StatusCodeKey = "StatusCode";

    public static Success WithStatusCode(this Success success, int statusCode)
    {
        return success.WithMetadata(StatusCodeKey, statusCode);
    }
    public static bool TryGetStatusCode(this Success success, out int statusCode)
    {
        if (success.Metadata.TryGetValue(StatusCodeKey, out object? value) &&
            value is int intValue)
        {
            statusCode = intValue;
            return true;
        }
        statusCode = default;
        return false;
    }

    public static int? GetStatusCode(this Success success)
    {
        return success.TryGetStatusCode(out int statusCode)
            ? statusCode
            : null;
    }

}
