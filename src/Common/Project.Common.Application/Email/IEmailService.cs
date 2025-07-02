namespace Project.Common.Application.Email;

public interface IEmailService
{
    Task<bool> SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
    Task<bool> SendTemplateAsync(string to, string subject, string templatePath, object model, CancellationToken cancellationToken = default);
}
