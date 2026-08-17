namespace Accounting.Api.Common
{
    /// <summary>
    /// Імена політик обмеження частоти. Константа, а не рядок у двох місцях:
    /// друкарська помилка в атрибуті мовчки лишила б ендпоінт без захисту.
    /// </summary>
    public static class RateLimitPolicies
    {
        public const string PublicClientRequests = "public-client-requests";

        /// <summary>
        /// Окремо від заявок навмисно: у кожної форми має бути свій лічильник.
        /// Спільний означав би, що кілька підписок вичерпують право надіслати
        /// заявку — а це найцінніша дія на сайті.
        /// </summary>
        public const string PublicNewsletter = "public-newsletter";
    }
}
