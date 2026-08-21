using System;

namespace Accounting.Application.Abstractions.News
{
    /// <summary>
    /// A single article from the source feed.
    ///
    /// Deliberately only title, short summary and link: the full text belongs to
    /// the source and we have no right to copy it. That is how feeds are meant
    /// to work: the teaser here, the reading there.
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
