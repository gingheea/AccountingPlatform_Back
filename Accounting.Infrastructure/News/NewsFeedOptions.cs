namespace Accounting.Infrastructure.News
{
    public sealed class NewsFeedOptions
    {
        public const string SectionName = "NewsFeed";

        /// <summary>Feed URL. Both formats are supported: RSS and Atom.</summary>
        public string Url { get; init; } = "https://news.dtkt.ua/rss";

        /// <summary>Source name, shown on the card so the attribution is visible.</summary>
        public string SourceName { get; init; } = "Дебет-Кредит";

        /// <summary>
        /// How many minutes to keep the response in memory without calling the source.
        /// News does not change by the second, and needless calls mean both a slow
        /// page and a risk of the source blocking us.
        /// </summary>
        public int CacheMinutes { get; init; } = 30;

        public int TimeoutSeconds { get; init; } = 15;
    }
}
