using Accounting.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Newsletter.ListSubscribers
{
    public sealed class ListSubscribersHandler
        : IRequestHandler<ListSubscribersQuery, IReadOnlyList<NewsletterSubscriberDto>>
    {
        private readonly INewsletterSubscriberRepository _repository;

        public ListSubscribersHandler(INewsletterSubscriberRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<NewsletterSubscriberDto>> Handle(
            ListSubscribersQuery request,
            CancellationToken ct)
        {
            var query = _repository.Query().AsNoTracking();

            if (request.IsActive is not null)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            return await query
                .OrderByDescending(x => x.SubscribedAtUtc)
                .Select(x => new NewsletterSubscriberDto(
                    x.Id, x.Email, x.Source, x.IsActive, x.SubscribedAtUtc, x.UnsubscribedAtUtc))
                .ToListAsync(ct);
        }
    }
}
