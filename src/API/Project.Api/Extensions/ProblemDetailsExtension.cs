namespace Project.Api.Extensions;

internal static class ProblemDetailsExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddProblemDetailsWithExtensions()
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
}