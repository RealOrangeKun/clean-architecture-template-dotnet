using Microsoft.OpenApi;

namespace Project.Api.Extensions;

internal static class OpenApiExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddOpenApiDocumentation()
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new()
                    {
                        Title = "Project API",
                        Version = "v1",
                        Description = "Project API Documentation",
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
                    };

                    if (document.Paths is not null)
                    {
                        foreach (OpenApiPathItem path in document.Paths.Values.Cast<OpenApiPathItem>())
                        {
                            if (path.Operations is not null)
                            {
                                foreach (OpenApiOperation operation in path.Operations.Values)
                                {
                                    operation.Security ??= [];
                                    operation.Security.Add(new()
                                    {
                                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                                    });
                                }
                            }
                        }
                    }

                    return Task.CompletedTask;
                });
            });

            return services;
        }
    }
}

public static class ApplicationBuilderExtensions
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseSwaggerUIWithOpenApi()
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Project API v1");
                options.RoutePrefix = "swagger";
            });
            return app;
        }
    }
}