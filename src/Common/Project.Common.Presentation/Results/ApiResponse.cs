namespace Project.Common.Presentation.Results;

public sealed class ApiResponse<T>
{
    public bool Success { get; }
    public string Message { get; }
    public T? Data { get; }
    public IEnumerable<string> Errors { get; private set; } = [];

    private ApiResponse(bool success, string message, T? data, IEnumerable<string>? errors = null)
    {
        Success = success;
        Message = message;
        Data = data;
        if (errors is not null)
        {
            Errors = errors;
        }
    }

    public static ApiResponse<T> Create(bool success, string message, T? data = default, IEnumerable<string>? errors = null)
    {
        return new ApiResponse<T>(success, message, data, errors);
    }
}

public sealed class ApiResponse
{
    public bool Success { get; private set; }
    public string Message { get; private set; }
    public IEnumerable<string> Errors { get; private set; } = [];

    private ApiResponse(bool success, string message, IEnumerable<string>? errors = null)
    {
        Success = success;
        Message = message;
        if (errors is not null)
        {
            Errors = errors;
        }
    }

    public static ApiResponse Create(bool success, string message, IEnumerable<string>? errors = null)
    {
        return new ApiResponse(success, message, errors);
    }

}
