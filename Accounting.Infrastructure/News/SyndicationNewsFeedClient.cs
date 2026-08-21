using Accounting.Application.Abstractions.News;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Accounting.Infrastructure.News
{
    internal sealed class SyndicationNewsFeedClient : INewsFeedClient
    {
        private const string CacheKey = "news-feed";

        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly NewsFeedOptions _options;
        private readonly ILogger<SyndicationNewsFeedClient> _logger;

        public SyndicationNewsFeedClient(
            HttpClient httpClient,
            IMemoryCache cache,
            IOptions<NewsFeedOptions> options,
            ILogger<SyndicationNewsFeedClient> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<IReadOnlyList<NewsArticle>> GetLatestAsync(int take, CancellationToken ct)
        {
            // The cache holds the whole list and only then we slice off what was asked.
            // Otherwise asking for 3 and for 9 articles would be two different cache keys.
            var all = await GetOrLoadAsync(ct);

            return all.Take(take).ToList();
        }

        private async Task<IReadOnlyList<NewsArticle>> GetOrLoadAsync(CancellationToken ct)
        {
            if (_cache.TryGetValue(CacheKey, out IReadOnlyList<NewsArticle>? cached) && cached is not null)
                return cached;

            IReadOnlyList<NewsArticle> articles;

            try
            {
                articles = await LoadFromSourceAsync(ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // A third-party source can be down, return garbage or simply not answer.
                // That is no reason to break our page: return an empty list and let the
                // frontend show "news temporarily unavailable".
                _logger.LogError(ex, "Failed to load the news feed from {Url}.", _options.Url);

                // A short pause before the next attempt, so the source is not hit on every
                // browser refresh.
                _cache.Set(CacheKey, Array.Empty<NewsArticle>() as IReadOnlyList<NewsArticle>,
                    TimeSpan.FromMinutes(2));

                return Array.Empty<NewsArticle>();
            }

            _cache.Set(CacheKey, articles, TimeSpan.FromMinutes(_options.CacheMinutes));

            return articles;
        }

        private async Task<IReadOnlyList<NewsArticle>> LoadFromSourceAsync(CancellationToken ct)
        {
            await using var stream = await _httpClient.GetStreamAsync(_options.Url, ct);

            // DtdProcessing = Prohibit closes off XXE, the classic attack where foreign
            // XML declares an entity that reads a file from our disk.
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                Async = false
            };

            using var reader = XmlReader.Create(stream, settings);

            // The same Load parses both <rss><item> and <feed><entry>.
            // That is exactly why System.ServiceModel.Syndication is used here:
            // by hand it would take two separate parsers.
            var feed = SyndicationFeed.Load(reader);

            if (feed is null)
                throw new InvalidOperationException("The feed could not be parsed.");

            return feed.Items
                .Select(ToArticle)
                .Where(article => article is not null)
                .Select(article => article!)
                .OrderByDescending(article => article.PublishedAtUtc)
                .ToList();
        }

        private NewsArticle? ToArticle(SyndicationItem item)
        {
            var url = item.Links.FirstOrDefault()?.Uri?.ToString();
            var title = Clean(item.Title?.Text);

            // Without a title or a link the card is useless: better to skip the entry
            // than to show an empty rectangle.
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(title))
                return null;

            var summary = Clean(item.Summary?.Text)
                ?? Clean((item.Content as TextSyndicationContent)?.Text)
                ?? string.Empty;

            // In RSS the date sits in PublishDate, in Atom usually in LastUpdatedTime.
            var published = item.PublishDate != default
                ? item.PublishDate
                : item.LastUpdatedTime;

            return new NewsArticle(
                Title: title,
                Summary: Shorten(summary, 220),
                Url: url,
                PublishedAtUtc: published.ToUniversalTime(),
                Category: ResolveCategory(item, url),
                Source: _options.SourceName);
        }

        /// <summary>
        /// Feed summaries arrive with HTML markup inside. Showing it as text is
        /// wrong, and injecting it as HTML is worse: it is foreign content and
        /// could carry a script.
        /// </summary>
        private static string? Clean(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return null;

            var withoutTags = Regex.Replace(raw, "<.*?>", " ");
            var decoded = WebUtility.HtmlDecode(withoutTags);
            var collapsed = Regex.Replace(decoded, @"\s+", " ").Trim();

            return collapsed.Length == 0 ? null : collapsed;
        }

        private static string Shorten(string text, int maxLength)
        {
            if (text.Length <= maxLength)
                return text;

            // Cut at the last space so a word is not chopped in half.
            var cut = text.LastIndexOf(' ', maxLength - 1);

            if (cut < maxLength / 2)
                cut = maxLength - 1;

            return text[..cut].TrimEnd(',', '.', ';', ':') + "…";
        }

        private static readonly Dictionary<string, string> CategoryNames = new(StringComparer.OrdinalIgnoreCase)
        {
            ["taxation"] = "Податки",
            ["accounting"] = "Облік",
            ["labor"] = "Праця",
            ["state"] = "Держава",
            ["law"] = "Право",
            ["finance"] = "Фінанси",
            ["simple"] = "Спрощенка"
        };

        /// <summary>
        /// The feed sets no &lt;category&gt;, but the section shows in the URL:
        /// news.dtkt.ua/<b>taxation</b>/pdv/113916-... We take the first segment.
        /// </summary>
        private static string ResolveCategory(SyndicationItem item, string url)
        {
            var declared = item.Categories.FirstOrDefault()?.Name;

            if (!string.IsNullOrWhiteSpace(declared))
                return declared;

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                var firstSegment = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault();

                if (firstSegment is not null && CategoryNames.TryGetValue(firstSegment, out var name))
                    return name;
            }

            return "Новини";
        }
    }
}
