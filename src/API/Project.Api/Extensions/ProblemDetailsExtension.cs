
namespace Project.Api.Extensions;

internal static class ProblemDetailsExtension
{
    internal static IServiceCollection AddProblemDetailsWithExtensions(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });

        return services;
    }
}
