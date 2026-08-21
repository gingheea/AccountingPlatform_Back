using Accounting.Application.Abstractions.Messaging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Messaging
{
    internal sealed class BrevoApiEmailSender : IEmailSender
    {
        private const string SendEndpoint = "https://api.brevo.com/v3/smtp/email";

        private readonly HttpClient _httpClient;
        private readonly BrevoOptions _options;

        public BrevoApiEmailSender(HttpClient httpClient, IOptions<BrevoOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task SendAsync(EmailMessage message, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                throw new InvalidOperationException("Brevo:ApiKey is not configured.");

            if (string.IsNullOrWhiteSpace(message.To))
                throw new InvalidOperationException("Email recipient is empty.");

            var payload = new Dictionary<string, object?>
            {
                ["sender"] = new { name = _options.FromName, email = _options.FromEmail },
                ["to"] = new[] { new { email = message.To } },
                ["subject"] = message.Subject,
                ["htmlContent"] = message.HtmlBody
            };

            if (!string.IsNullOrWhiteSpace(message.TextBody))
                payload["textContent"] = message.TextBody;

            if (!string.IsNullOrWhiteSpace(message.ReplyTo))
                payload["replyTo"] = new { email = message.ReplyTo, name = message.ReplyToName ?? string.Empty };

            using var response = await _httpClient.PostAsJsonAsync(SendEndpoint, payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                // Brevo's response body carries the reason for the refusal; without it
                // the logs would show a bare status code and nothing else.
                var body = await response.Content.ReadAsStringAsync(ct);

                throw new InvalidOperationException(
                    $"Brevo rejected the email ({(int)response.StatusCode}): {body}");
            }
        }
    }
}
