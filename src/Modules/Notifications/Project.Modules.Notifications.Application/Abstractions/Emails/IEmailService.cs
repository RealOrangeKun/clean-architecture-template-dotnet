namespace Project.Modules.Notifications.Application.Abstractions.Emails;

public interface IEmailService
{
    Task<bool> SendAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default);

    Task<bool> SendTemplateAsync(
        string to,
        string subject,
        string templatePath,
        object model,
        CancellationToken cancellationToken = default);

    Task<bool> SendTemplateAsync<TTemplate>(
        TTemplate template,
        CancellationToken cancellationToken = default)
        where TTemplate : IEmailTemplate;
}
