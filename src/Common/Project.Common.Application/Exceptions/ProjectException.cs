using FluentResults;

namespace Project.Common.Application.Exceptions;

public sealed class ProjectException : Exception
{
    public ProjectException(string requestName, Error? error = default, Exception? innerException = default)
        : base("Application exception occurred.", innerException)
    {
        RequestName = requestName;
        Error = error;
    }

    public string RequestName { get; }
    public Error? Error { get; }
}
