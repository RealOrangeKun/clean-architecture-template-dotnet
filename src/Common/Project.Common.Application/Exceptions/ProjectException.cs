using FluentResults;

namespace Project.Common.Application.Exceptions;

public sealed class ProjectException(string requestName, Error? error = default, Exception? innerException = default) : Exception("Application exception occurred.", innerException)
{
    public string RequestName { get; } = requestName;
    public Error? Error { get; } = error;
}
