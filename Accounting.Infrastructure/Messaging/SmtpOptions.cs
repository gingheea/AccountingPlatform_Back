namespace Accounting.Infrastructure.Messaging
{
    /// <summary>
    /// Тільки транспорт: як під'єднатись до relay і від чийого імені слати.
    /// Кому саме слати — не тут, це в NotificationOptions на рівні Application.
    /// </summary>
    public class SmtpOptions
    {
        public const string SectionName = "Smtp";

        public string Host { get; init; } = string.Empty;
        public int Port { get; init; } = 587;
        public string User { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string FromEmail { get; init; } = string.Empty;
        public string FromName { get; init; } = string.Empty;

        /// <summary>Скільки чекати на relay, поки не здатись. Відправка синхронна, тож клієнт чекає разом з нами.</summary>
        public int TimeoutSeconds { get; init; } = 10;
    }
}
