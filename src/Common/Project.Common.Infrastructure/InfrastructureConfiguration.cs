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
using FluentEmail.Core.Interfaces;
using FluentEmail.Smtp;

namespace Project.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string databaseConnectionString,
        string redisConnectionString,
        string fromEmail)
    {
        services.AddAuthenticationInternal();

        services.AddNpgsqlDataSource(databaseConnectionString);

        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.TryAddSingleton<DomainEventsDispatcherInterceptor>();

        services.AddEmailServices(fromEmail);

        services.AddQuartz();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddCachingInternal(redisConnectionString);

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }

    private static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        string fromEmail)
    {
        services.AddOptions<FluentEmailOptions>()
            .BindConfiguration(FluentEmailOptions.SectionName);

        services.TryAddScoped<ISmtpClientFactory, SmtpClientFactory>();

        services.AddFluentEmail(fromEmail)
            .AddRazorRenderer();

        services.AddScoped<ISender>(sp =>
        {
            ISmtpClientFactory factory = sp.GetRequiredService<ISmtpClientFactory>();
            SmtpClient client = factory.Create();
            return new SmtpSender(client);
        });

        services.TryAddScoped<IEmailService, EmailService>();

        return services;
    }
}
