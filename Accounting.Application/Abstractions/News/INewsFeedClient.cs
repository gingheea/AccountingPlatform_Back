using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.News
{
    /// <summary>
    /// Application не знає ні адреси стрічки, ні її формату — лише те, що
    /// звідкись беруться новини. Зміна джерела не зачепить ані обробник,
    /// ані контролер.
    /// </summary>
    public interface INewsFeedClient
    {
        Task<IReadOnlyList<NewsArticle>> GetLatestAsync(int take, CancellationToken ct);
    }
}
