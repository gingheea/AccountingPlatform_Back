namespace Accounting.Infrastructure.News
{
    public sealed class NewsFeedOptions
    {
        public const string SectionName = "NewsFeed";

        /// <summary>Адреса стрічки. Підтримуються обидва формати — RSS і Atom.</summary>
        public string Url { get; init; } = "https://news.dtkt.ua/rss";

        /// <summary>Назва джерела — показуємо її на картці, щоб авторство було видно.</summary>
        public string SourceName { get; init; } = "Дебет-Кредит";

        /// <summary>
        /// Скільки хвилин тримати відповідь у памʼяті, не ходячи до джерела.
        /// Новини не оновлюються щосекунди, а зайві запити — це і повільна
        /// сторінка, і ризик, що джерело почне нас блокувати.
        /// </summary>
        public int CacheMinutes { get; init; } = 30;

        public int TimeoutSeconds { get; init; } = 15;
    }
}
