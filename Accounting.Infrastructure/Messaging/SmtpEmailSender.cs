using Accounting.Application.Abstractions.Messaging;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Messaging
{
    internal sealed class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;

        public SmtpEmailSender(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(EmailMessage message, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(_options.Host))
                throw new InvalidOperationException("Smtp:Host is not configured.");

            if (string.IsNullOrWhiteSpace(message.To))
                throw new InvalidOperationException("Email recipient is empty.");

            var mime = BuildMessage(message);

            using var client = new SmtpClient
            {
                Timeout = _options.TimeoutSeconds * 1000
            };

            // Порт 587 = відкрите з'єднання, яке командою STARTTLS піднімається
            // до шифрованого. Для порту 465 тут був би SslOnConnect.
            await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(_options.User, _options.Password, ct);
            await client.SendAsync(mime, ct);
            await client.DisconnectAsync(quit: true, ct);
        }

        private MimeMessage BuildMessage(EmailMessage message)
        {
            var mime = new MimeMessage();

            // From мусить збігатися з підтвердженим у Brevo відправником, інакше relay відмовить.
            mime.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
            mime.To.Add(MailboxAddress.Parse(message.To));

            if (!string.IsNullOrWhiteSpace(message.ReplyTo))
                mime.ReplyTo.Add(new MailboxAddress(message.ReplyToName ?? string.Empty, message.ReplyTo));

            mime.Subject = message.Subject;

            var body = new BodyBuilder
            {
                HtmlBody = message.HtmlBody,
                TextBody = message.TextBody
            };

            mime.Body = body.ToMessageBody();

            return mime;
        }
    }
}
