using Accounting.Application.Abstractions.News;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.News.GetLatestNews
{
    public sealed class GetLatestNewsHandler
        : IRequestHandler<GetLatestNewsQuery, IReadOnlyList<NewsArticleDto>>
    {
        private const int MaxTake = 30;

        private readonly INewsFeedClient _feedClient;

        public GetLatestNewsHandler(INewsFeedClient feedClient)
        {
            _feedClient = feedClient;
        }

        public async Task<IReadOnlyList<NewsArticleDto>> Handle(
            GetLatestNewsQuery request,
            CancellationToken ct)
        {
            // Take arrives in the query string, meaning anyone outside controls it.
            // We cap it ourselves so ?take=100000 is not a way to load us down.
            var take = Math.Clamp(request.Take, 1, MaxTake);

            var articles = await _feedClient.GetLatestAsync(take, ct);

            return articles
                .Select(a => new NewsArticleDto(
                    a.Title, a.Summary, a.Url, a.PublishedAtUtc, a.Category, a.Source))
                .ToList();
        }
    }
}
