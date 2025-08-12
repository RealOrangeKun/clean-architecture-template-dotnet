using System.Net.Mail;

namespace Project.Common.Infrastructure.Email;

public interface ISmtpClientFactory
{
    SmtpClient Create();
}
