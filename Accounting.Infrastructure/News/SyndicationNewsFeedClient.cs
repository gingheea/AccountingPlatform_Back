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
            // Кеш зберігає весь список, а вже потім відрізаємо потрібну кількість.
            // Інакше запит на 3 новини й на 9 були б різними ключами кешу.
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
                // Чуже джерело може лежати, віддати сміття чи просто не відповісти.
                // Це не привід валити нашу сторінку — повертаємо порожній список,
                // а фронт покаже «новини тимчасово недоступні».
                _logger.LogError(ex, "Failed to load the news feed from {Url}.", _options.Url);

                // Коротка пауза перед наступною спробою, щоб не бити джерело
                // на кожен перезавантажений браузером запит.
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

            // DtdProcessing = Prohibit закриває XXE — класичну атаку, коли чужий
            // XML описує сутність, що читає файл з нашого диска.
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                Async = false
            };

            using var reader = XmlReader.Create(stream, settings);

            // Одна й та сама Load розбирає і <rss><item>, і <feed><entry>.
            // Саме заради цього взято System.ServiceModel.Syndication —
            // руками довелося б писати два різні парсери.
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

            // Без заголовка чи посилання картка непридатна — краще пропустити запис,
            // ніж показати порожній прямокутник.
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(title))
                return null;

            var summary = Clean(item.Summary?.Text)
                ?? Clean((item.Content as TextSyndicationContent)?.Text)
                ?? string.Empty;

            // У RSS дата лежить у PublishDate, в Atom частіше в LastUpdatedTime.
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
        /// Описи у стрічках приходять з HTML-розміткою всередині. Показувати її
        /// як текст не можна, вставляти на сторінку як HTML — тим паче: це чужий
        /// вміст, і він міг би принести скрипт.
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

            // Ріжемо по останньому пробілу, щоб не обірвати слово посередині.
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
        /// Стрічка не проставляє &lt;category&gt;, але рубрика видно в адресі:
        /// news.dtkt.ua/<b>taxation</b>/pdv/113916-... Беремо перший сегмент.
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
