namespace Project.Common.Domain;

public enum ErrorType
{
    Failure,
    Validation,
    Problem,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    TooManyRequests
}
