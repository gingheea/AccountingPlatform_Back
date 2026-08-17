using Accounting.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Messaging
{
    internal sealed class BrevoContactClient : INewsletterContactClient
    {
        private const string ContactsEndpoint = "https://api.brevo.com/v3/contacts";

        private readonly HttpClient _httpClient;
        private readonly BrevoOptions _options;
        private readonly ILogger<BrevoContactClient> _logger;

        public BrevoContactClient(
            HttpClient httpClient,
            IOptions<BrevoOptions> options,
            ILogger<BrevoContactClient> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task AddContactAsync(string email, CancellationToken ct)
        {
            if (_options.NewsletterListId is null or <= 0)
            {
                // Списку немає — підписка все одно збережена в нашій базі,
                // тож це попередження, а не помилка.
                _logger.LogWarning(
                    "Brevo:NewsletterListId is not configured, {Email} was saved locally only.", email);

                return;
            }

            var payload = new Dictionary<string, object?>
            {
                ["email"] = email,
                ["listIds"] = new[] { _options.NewsletterListId.Value },
                // Щоб повторна підписка не падала з «контакт уже існує».
                ["updateEnabled"] = true
            };

            using var response = await _httpClient.PostAsJsonAsync(ContactsEndpoint, payload, ct);

            // 204 — контакт оновлено, 201 — створено. Обидва нас влаштовують.
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
                return;

            var body = await response.Content.ReadAsStringAsync(ct);

            throw new InvalidOperationException(
                $"Brevo rejected the contact ({(int)response.StatusCode}): {body}");
        }
    }
}
