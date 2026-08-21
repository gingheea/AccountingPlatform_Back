namespace Accounting.Api.Common
{
    /// <summary>
    /// Rate limiting policy names. A constant rather than a literal in two places:
    /// a typo in the attribute would silently leave an endpoint unprotected.
    /// </summary>
    public static class RateLimitPolicies
    {
        public const string PublicClientRequests = "public-client-requests";

        /// <summary>
        /// Deliberately separate from requests: every form needs its own counter.
        /// A shared one would mean a few newsletter signups use up the right to send
        /// a request, which is the most valuable action on the site.
        /// </summary>
        public const string PublicNewsletter = "public-newsletter";
    }
}
