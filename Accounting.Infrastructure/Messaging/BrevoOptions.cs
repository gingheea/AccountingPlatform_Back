namespace Accounting.Infrastructure.Messaging
{
    /// <summary>
    /// Відправка через HTTP API Brevo замість SMTP: безкоштовні хостинги
    /// (зокрема Render) блокують вихідний порт 587, а 443 відкритий завжди.
    /// </summary>
    public class BrevoOptions
    {
        public const string SectionName = "Brevo";

        /// <summary>Ключ із вкладки <b>API Keys</b>, а не SMTP key — це різні ключі.</summary>
        public string ApiKey { get; init; } = string.Empty;

        /// <summary>Має збігатися з підтвердженим відправником у Brevo.</summary>
        public string FromEmail { get; init; } = string.Empty;

        public string FromName { get; init; } = string.Empty;

        public int TimeoutSeconds { get; init; } = 15;
    }
}
