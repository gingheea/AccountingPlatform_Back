using Accounting.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Persistence
{
    public interface INewsletterSubscriberRepository
    {
        Task<NewsletterSubscriber?> GetByEmailAsync(string email, CancellationToken ct);

        Task AddAsync(NewsletterSubscriber subscriber, CancellationToken ct);

        void Remove(NewsletterSubscriber subscriber);

        IQueryable<NewsletterSubscriber> Query();
    }
}
