namespace Accounting.Infrastructure.Messaging
{
    /// <summary>
    /// Sending through Brevo's HTTP API instead of SMTP: free hosts (Render
    /// among them) block outbound port 587, while 443 is always open.
    /// </summary>
    public class BrevoOptions
    {
        public const string SectionName = "Brevo";

        /// <summary>The key from the <b>API Keys</b> tab, not the SMTP key: they differ.</summary>
        public string ApiKey { get; init; } = string.Empty;

        /// <summary>Must match a verified sender in Brevo.</summary>
        public string FromEmail { get; init; } = string.Empty;

        public string FromName { get; init; } = string.Empty;

        public int TimeoutSeconds { get; init; } = 15;

        /// <summary>
        /// Brevo mailing list id (Contacts, then Lists). When unset, subscribers
        /// are stored only in our database, without failing.
        /// </summary>
        public int? NewsletterListId { get; init; }
    }
}
