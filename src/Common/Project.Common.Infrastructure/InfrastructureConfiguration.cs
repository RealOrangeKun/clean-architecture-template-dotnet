using System.Net;
using System.Net.Mail;
using Project.Common.Application.Caching;
using Project.Common.Application.Data;
using Project.Common.Application.Email;
using Project.Common.Infrastructure.Authentication;
using Project.Common.Infrastructure.Caching;
using Project.Common.Infrastructure.Data;
using Project.Common.Infrastructure.Email;
using Project.Common.Infrastructure.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Npgsql;
using Quartz;

namespace Project.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string databaseConnectionString,
        string redisConnectionString)
    {
        services.AddAuthenticationInternal();

        services.AddNpgsqlDataSource(databaseConnectionString);

        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.TryAddSingleton<DomainEventsDispatcherInterceptor>();

        services.AddOptions<FluentEmailOptions>()
            .BindConfiguration("FluentEmail");

        ServiceProvider provider = services.BuildServiceProvider();
        FluentEmailOptions options = provider.GetRequiredService<IOptions<FluentEmailOptions>>().Value;
        services.AddFluentEmail(options.From)
            .AddRazorRenderer()
            .AddSmtpSender(() =>
            {
                FluentEmailOptions smtpOptions = provider.GetRequiredService<IOptions<FluentEmailOptions>>().Value;
                var client = new SmtpClient(smtpOptions.Host, smtpOptions.Port)
                {
                    Credentials = new NetworkCredential(smtpOptions.Username, smtpOptions.Password),
                    EnableSsl = true
                };
                return client;
            });

        services.TryAddScoped<IEmailService, EmailService>();

        services.AddQuartz();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddCachingInternal(redisConnectionString);

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}
