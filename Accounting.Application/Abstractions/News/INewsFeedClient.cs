using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.News
{
    /// <summary>
    /// Application knows neither the feed URL nor its format, only that news
    /// come from somewhere. Changing the source touches neither the handler
    /// nor the controller.
    /// </summary>
    public interface INewsFeedClient
    {
        Task<IReadOnlyList<NewsArticle>> GetLatestAsync(int take, CancellationToken ct);
    }
}
