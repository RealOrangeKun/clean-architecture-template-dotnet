using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Project.Common.Infrastructure.Email;

internal sealed class SmtpClientFactory(IOptions<FluentEmailOptions> options) : ISmtpClientFactory
{
    private readonly FluentEmailOptions _options = options.Value;

    public SmtpClient Create()
    {
        return new SmtpClient(_options.Host, _options.Port)
        {
            Credentials = new NetworkCredential(_options.Username, _options.Password),
            EnableSsl = true
        };
    }
}
