using Accounting.Application.Abstractions.Messaging;
using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common.Options;
using Accounting.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.Events
{
    public sealed class SendNewClientRequestEmailHandler
        : INotificationHandler<ClientRequestCreatedNotification>
    {
        private readonly IEmailSender _emailSender;
        private readonly IServiceRepository _serviceRepository;
        private readonly IPricingPackageRepository _pricingPackageRepository;
        private readonly NotificationOptions _options;
        private readonly ILogger<SendNewClientRequestEmailHandler> _logger;

        public SendNewClientRequestEmailHandler(
            IEmailSender emailSender,
            IServiceRepository serviceRepository,
            IPricingPackageRepository pricingPackageRepository,
            IOptions<NotificationOptions> options,
            ILogger<SendNewClientRequestEmailHandler> logger)
        {
            _emailSender = emailSender;
            _serviceRepository = serviceRepository;
            _pricingPackageRepository = pricingPackageRepository;
            _options = options.Value;
            _logger = logger;
        }

        public async Task Handle(ClientRequestCreatedNotification notification, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(_options.AccountantEmail))
            {
                _logger.LogWarning(
                    "Notifications:AccountantEmail is not configured, skipping email for request {RequestId}.",
                    notification.RequestId);

                return;
            }

            try
            {
                var subject = await BuildSubjectAsync(notification, ct);

                await _emailSender.SendAsync(
                    new EmailMessage(
                        To: _options.AccountantEmail,
                        Subject: subject,
                        HtmlBody: BuildHtmlBody(notification, subject),
                        TextBody: BuildTextBody(notification),
                        // Бухгалтер тисне «Відповісти» — і пише одразу клієнту.
                        ReplyTo: notification.Email,
                        ReplyToName: notification.FullName),
                    ct);

                _logger.LogInformation(
                    "New client request email sent for {RequestId}.", notification.RequestId);
            }
            catch (Exception ex)
            {
                // Заявка вже збережена. Якщо тут прокинути виняток далі, MediatR
                // поверне його в CreateClientRequestHandler, клієнт побачить 500
                // і надішле форму ще раз — у базі з'явиться дубль.
                _logger.LogError(
                    ex,
                    "Failed to send new client request email for {RequestId}.",
                    notification.RequestId);
            }
        }

        private async Task<string> BuildSubjectAsync(
            ClientRequestCreatedNotification notification,
            CancellationToken ct)
        {
            var subject = notification.RequestType switch
            {
                RequestType.Service => await ServiceNameAsync(notification.ServiceId, ct),
                RequestType.Package => await PackageNameAsync(notification.PricingPackageId, ct),
                _ => "загальна консультація"
            };

            return $"Нова заявка: {notification.FullName} — {subject}";
        }

        private async Task<string> ServiceNameAsync(Guid? id, CancellationToken ct)
        {
            if (id is null) return "послуга не вказана";

            var service = await _serviceRepository.GetByIdAsync(id.Value, ct);

            return service?.Name ?? "послуга не знайдена";
        }

        private async Task<string> PackageNameAsync(Guid? id, CancellationToken ct)
        {
            if (id is null) return "пакет не вказаний";

            var package = await _pricingPackageRepository.GetByIdAsync(id.Value, ct);

            return package?.Name ?? "пакет не знайдено";
        }

        private string BuildHtmlBody(ClientRequestCreatedNotification n, string subject)
        {
            var rows = new StringBuilder();

            AppendRow(rows, "Ім'я", n.FullName);
            AppendRow(rows, "Email", n.Email);
            AppendRow(rows, "Телефон", n.Phone ?? "не вказано");
            AppendRow(rows, "Тип заявки", TypeLabel(n.RequestType));
            AppendRow(rows, "Отримано", n.CreatedAtUtc.ToString("dd.MM.yyyy HH:mm") + " UTC");

            var message = string.IsNullOrWhiteSpace(n.Message)
                ? "<p style=\"color:#6b7280\">Без повідомлення</p>"
                : $"<p style=\"white-space:pre-wrap\">{Encode(n.Message)}</p>";

            var link = string.IsNullOrWhiteSpace(_options.AdminRequestsUrl)
                ? string.Empty
                : $"<p style=\"margin-top:24px\"><a href=\"{Encode(_options.AdminRequestsUrl)}\" " +
                  "style=\"background:#1e3a5f;color:#fff;padding:12px 20px;border-radius:8px;" +
                  "text-decoration:none;display:inline-block\">Відкрити в адмін-панелі</a></p>";

            return $$"""
                <div style="font-family:Arial,Helvetica,sans-serif;color:#111827;max-width:600px">
                  <h2 style="margin:0 0 16px">{{Encode(subject)}}</h2>
                  <table cellpadding="6" cellspacing="0" style="border-collapse:collapse;width:100%">
                    {{rows}}
                  </table>
                  <h3 style="margin:24px 0 8px">Повідомлення</h3>
                  {{message}}
                  {{link}}
                </div>
                """;
        }

        private string BuildTextBody(ClientRequestCreatedNotification n)
        {
            var text = new StringBuilder();

            text.AppendLine("Нова заявка з сайту.");
            text.AppendLine();
            text.AppendLine($"Ім'я: {n.FullName}");
            text.AppendLine($"Email: {n.Email}");
            text.AppendLine($"Телефон: {n.Phone ?? "не вказано"}");
            text.AppendLine($"Тип заявки: {TypeLabel(n.RequestType)}");
            text.AppendLine($"Отримано: {n.CreatedAtUtc:dd.MM.yyyy HH:mm} UTC");
            text.AppendLine();
            text.AppendLine("Повідомлення:");
            text.AppendLine(string.IsNullOrWhiteSpace(n.Message) ? "(без повідомлення)" : n.Message);

            if (!string.IsNullOrWhiteSpace(_options.AdminRequestsUrl))
            {
                text.AppendLine();
                text.AppendLine(_options.AdminRequestsUrl);
            }

            return text.ToString();
        }

        private static void AppendRow(StringBuilder rows, string label, string value) =>
            rows.Append(
                $"<tr><td style=\"border-bottom:1px solid #e5e7eb;color:#6b7280;width:140px\">{Encode(label)}</td>" +
                $"<td style=\"border-bottom:1px solid #e5e7eb;font-weight:600\">{Encode(value)}</td></tr>");

        private static string TypeLabel(RequestType type) => type switch
        {
            RequestType.Service => "конкретна послуга",
            RequestType.Package => "пакет супроводу",
            _ => "загальна консультація"
        };

        // Дані прийшли з публічної форми — без екранування хтось вставить у поле
        // імені HTML, і лист поїде зламаним або з чужим посиланням.
        private static string Encode(string value) => WebUtility.HtmlEncode(value);
    }
}
