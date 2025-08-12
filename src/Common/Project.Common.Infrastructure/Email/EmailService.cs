using FluentEmail.Core;
using FluentEmail.Core.Models;
using Project.Common.Application.Email;

namespace Project.Common.Infrastructure.Email;

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
