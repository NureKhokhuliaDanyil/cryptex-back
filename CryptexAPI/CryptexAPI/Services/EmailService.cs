using CryptexAPI.Models.Identity;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using CryptexAPI.Services.Interfaces;

namespace CryptexAPI.Services;

public class EmailService(IOptions<EmailSettings> options) : IEmailService
{
    private readonly EmailSettings _settings = options.Value;

    public Task SendEmail(string toEmail, string subject, string body)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_settings.Email),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort);
        client.Credentials = new NetworkCredential(_settings.Email, _settings.Password);
        client.EnableSsl = _settings.EnableSsl;

        client.Send(message);
        return Task.CompletedTask;
    }
}