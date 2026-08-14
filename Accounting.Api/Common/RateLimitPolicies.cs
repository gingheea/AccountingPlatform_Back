namespace Accounting.Api.Common
{
    /// <summary>
    /// Імена політик обмеження частоти. Константа, а не рядок у двох місцях:
    /// друкарська помилка в атрибуті мовчки лишила б ендпоінт без захисту.
    /// </summary>
    public static class RateLimitPolicies
    {
        public const string PublicClientRequests = "public-client-requests";
    }
}
