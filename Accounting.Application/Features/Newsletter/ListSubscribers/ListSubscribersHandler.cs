using Accounting.Application.Abstractions.Persistence;
using Accounting.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Newsletter.ListSubscribers
{
    public sealed class ListSubscribersHandler
        : IRequestHandler<ListSubscribersQuery, PagedResult<NewsletterSubscriberDto>>
    {
        private readonly INewsletterSubscriberRepository _repository;

        public ListSubscribersHandler(INewsletterSubscriberRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<NewsletterSubscriberDto>> Handle(
            ListSubscribersQuery request,
            CancellationToken ct)
        {
            var query = _repository.Query().AsNoTracking();

            if (request.IsActive is not null)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            return await query
                .OrderByDescending(x => x.SubscribedAtUtc)
                .ThenBy(x => x.Id)
                .ToPagedResultAsync(
                    q => q.Select(x => new NewsletterSubscriberDto(
                        x.Id, x.Email, x.Source, x.IsActive, x.SubscribedAtUtc, x.UnsubscribedAtUtc)),
                    request.Page,
                    request.PageSize,
                    ct);
        }
    }
}
