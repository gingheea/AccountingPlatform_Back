using MediatR;
using System;
using System.Collections.Generic;

namespace Accounting.Application.Features.News.GetLatestNews
{
    public sealed record NewsArticleDto(
        string Title,
        string Summary,
        string Url,
        DateTimeOffset PublishedAtUtc,
        string Category,
        string Source
    );

    public sealed record GetLatestNewsQuery(int Take)
        : IRequest<IReadOnlyList<NewsArticleDto>>;
}
