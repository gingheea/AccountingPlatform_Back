using Accounting.Application.Abstractions.Persistence;
using Accounting.Domain.Entities;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Repositories
{
    internal class NewsletterSubscriberRepository : INewsletterSubscriberRepository
    {
        private readonly AppDbContext _dbContext;

        public NewsletterSubscriberRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<NewsletterSubscriber?> GetByEmailAsync(string email, CancellationToken ct)
            => await _dbContext.NewsletterSubscribers.FirstOrDefaultAsync(x => x.Email == email, ct);

        public async Task AddAsync(NewsletterSubscriber subscriber, CancellationToken ct)
            => await _dbContext.NewsletterSubscribers.AddAsync(subscriber, ct);

        public void Remove(NewsletterSubscriber subscriber)
            => _dbContext.NewsletterSubscribers.Remove(subscriber);

        public IQueryable<NewsletterSubscriber> Query()
            => _dbContext.NewsletterSubscribers.AsQueryable();
    }
}
