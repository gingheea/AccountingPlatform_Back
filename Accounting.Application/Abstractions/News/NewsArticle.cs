using System;

namespace Accounting.Application.Abstractions.News
{
    /// <summary>
    /// Одна новина зі стрічки джерела.
    ///
    /// Свідомо зберігаємо лише заголовок, короткий опис і посилання: повний
    /// текст належить джерелу, і копіювати його ми не маємо права. Саме так
    /// стрічки й задумані — анонс у нас, читання в них.
    /// </summary>
    public sealed record NewsArticle(
        string Title,
        string Summary,
        string Url,
        DateTimeOffset PublishedAtUtc,
        string Category,
        string Source
    );
}
