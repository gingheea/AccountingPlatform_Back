namespace Accounting.Application.Common.Options
{
    /// <summary>
    /// "Who and where" is a business decision, not a transport one, so it lives
    /// here rather than in the mail options. Changing provider must not touch it.
    /// </summary>
    public sealed class NotificationOptions
    {
        public const string SectionName = "Notifications";

        /// <summary>The accountant's address: recipient of new-request notifications.</summary>
        public string AccountantEmail { get; init; } = string.Empty;

        /// <summary>Link to the admin request list so the email can jump straight there.</summary>
        public string AdminRequestsUrl { get; init; } = string.Empty;

        /// <summary>
        /// Site address used to build the password reset link. The backend does not
        /// know the frontend routes, so the base comes from configuration.
        /// </summary>
        public string SiteUrl { get; init; } = string.Empty;
    }
}
