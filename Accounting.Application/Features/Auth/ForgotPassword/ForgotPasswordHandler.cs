using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Messaging;
using Accounting.Application.Common.Options;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Auth.ForgotPassword
{
    public sealed class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IEmailSender _emailSender;
        private readonly NotificationOptions _options;
        private readonly ILogger<ForgotPasswordHandler> _logger;

        public ForgotPasswordHandler(
            IUserManagementService userManagementService,
            IEmailSender emailSender,
            IOptions<NotificationOptions> options,
            ILogger<ForgotPasswordHandler> logger)
        {
            _userManagementService = userManagementService;
            _emailSender = emailSender;
            _options = options.Value;
            _logger = logger;
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken ct)
        {
            var email = request.Email.Trim();

            var ticket = await _userManagementService.CreatePasswordResetTicketAsync(email, ct);

            if (ticket is null)
            {
                // Акаунта немає — мовчки виходимо. Назовні відповідь така сама,
                // як при успіху: інакше формою відновлення можна було б
                // перевіряти, чи зареєстрована конкретна людина.
                _logger.LogInformation("Password reset requested for unknown email.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_options.SiteUrl))
            {
                _logger.LogError("Notifications:SiteUrl is not configured, cannot build a reset link.");
                return;
            }

            var link = BuildResetLink(ticket);

            try
            {
                await _emailSender.SendAsync(
                    new EmailMessage(
                        To: ticket.Email,
                        Subject: "Відновлення пароля",
                        HtmlBody: BuildHtmlBody(ticket, link),
                        TextBody: BuildTextBody(ticket, link)),
                    ct);
            }
            catch (Exception ex)
            {
                // Мовчимо назовні з тієї ж причини: різна поведінка при збої
                // теж підказала б, що акаунт існує.
                _logger.LogError(ex, "Failed to send the password reset email.");
            }
        }

        /// <summary>
        /// Код від Identity містить символи на кшталт «+» і «/», які в адресному
        /// рядку тлумачаться інакше — найчастіше «+» перетворюється на пробіл,
        /// і код приходить назад зіпсованим. Тому кодуємо його у безпечний для
        /// URL вигляд, а фронт повертає рядок як є.
        /// </summary>
        private string BuildResetLink(PasswordResetTicket ticket)
        {
            var encodedToken = Base64UrlEncode(ticket.Token);
            var baseUrl = _options.SiteUrl.TrimEnd('/');

            return $"{baseUrl}/reset-password?email={Uri.EscapeDataString(ticket.Email)}&token={encodedToken}";
        }

        private static string Base64UrlEncode(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        private static string BuildHtmlBody(PasswordResetTicket ticket, string link)
        {
            var name = string.IsNullOrWhiteSpace(ticket.FullName)
                ? "Вітаємо"
                : $"Вітаємо, {WebUtility.HtmlEncode(ticket.FullName)}";

            return $$"""
                <div style="font-family:Arial,Helvetica,sans-serif;color:#111827;max-width:600px">
                  <h2 style="margin:0 0 16px">Відновлення пароля</h2>
                  <p>{{name}}!</p>
                  <p>Ви попросили встановити новий пароль до кабінету. Натисніть кнопку нижче.</p>
                  <p style="margin:24px 0">
                    <a href="{{WebUtility.HtmlEncode(link)}}"
                       style="background:#1e3a5f;color:#fff;padding:12px 20px;border-radius:8px;
                              text-decoration:none;display:inline-block">Встановити новий пароль</a>
                  </p>
                  <p style="color:#6b7280;font-size:14px">
                    Посилання діє обмежений час і спрацює лише один раз.
                    Якщо ви цього не робили — просто проігноруйте лист, пароль лишиться попереднім.
                  </p>
                </div>
                """;
        }

        private static string BuildTextBody(PasswordResetTicket ticket, string link)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Відновлення пароля");
            builder.AppendLine();
            builder.AppendLine("Ви попросили встановити новий пароль до кабінету.");
            builder.AppendLine("Перейдіть за посиланням:");
            builder.AppendLine(link);
            builder.AppendLine();
            builder.AppendLine("Посилання діє обмежений час і спрацює лише один раз.");
            builder.AppendLine("Якщо ви цього не робили — просто проігноруйте лист.");

            return builder.ToString();
        }
    }
}
