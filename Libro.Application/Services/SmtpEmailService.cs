using Libro.Application.DTOs;
using Libro.Application.ServicesInterfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace Libro.Application.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public SmtpEmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmail(string recipientEmail, string subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(InternetAddress.Parse($"{_smtpSettings.SenderName} <{_smtpSettings.SenderEmail}>"));
            email.To.Add(InternetAddress.Parse(recipientEmail));
            email.Subject = subject;
            email.Body = new TextPart("plain")
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
        }
    }
}
