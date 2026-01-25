using FluentEmail.Core;
using FluentEmail.Core.Models;
using Project.Modules.Notifications.Application.Abstractions.Emails;

namespace Project.Modules.Notifications.Infrastructure.Emails;

internal sealed class EmailService(IFluentEmailFactory fluentEmailFactory) : IEmailService
{
    public async Task<bool> SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        return await RetryAsync(async () =>
        {
            IFluentEmail fluentEmail = fluentEmailFactory.Create();
            SendResponse response = await fluentEmail
                .To(to)
                .Subject(subject)
                .Body(body, isHtml)
                .SendAsync(cancellationToken);


            return response.Successful;
        });
    }

    public async Task<bool> SendTemplateAsync(string to, string subject, string templatePath, object model, CancellationToken cancellationToken = default)
    {
        return await RetryAsync(async () =>
        {
            IFluentEmail fluentEmail = fluentEmailFactory.Create();
            SendResponse response = await fluentEmail
            .To(to)
            .Subject(subject)
            .UsingTemplateFromFile(templatePath, model)
            .SendAsync(cancellationToken);

            return response.Successful;
        });
    }

    public async Task<bool> SendTemplateAsync<TTemplate>(TTemplate template, CancellationToken cancellationToken = default)
        where TTemplate : IEmailTemplate
    {
        if (template == null)
            throw new ArgumentNullException(nameof(template));

        return await SendTemplateAsync(
            template.RecipientEmail,
            template.Subject,
            template.TemplatePath,
            template,
            cancellationToken);
    }

    private static async Task<bool> RetryAsync(Func<Task<bool>> action, int maxAttempts = 3, int delayMs = 2000)
    {
        int attempt = 0;
        while (attempt <= maxAttempts)
        {
            if (await action())
                return true;
            await Task.Delay(delayMs);
        }
        return false;
    }
}
