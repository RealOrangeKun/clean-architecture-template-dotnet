using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Project.Common.Application.Messaging;

namespace Project.Common.Application.Behaviors;

internal sealed class LoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        var stopWatch = Stopwatch.StartNew();

        logger.LogInformation("Handling {RequestName} {@Request}", requestName, request);

        try
        {
            TResponse? response = await next(cancellationToken);

            stopWatch.Stop();
            logger.LogInformation("Handled {RequestName} in {Elapsed}ms {@Response}",
                requestName, stopWatch.ElapsedMilliseconds, response);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling {RequestName} {@Request}", requestName, request);
            throw;
        }
    }
}


