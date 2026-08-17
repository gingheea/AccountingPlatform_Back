using Accounting.Application.Abstractions.Persistence;
using Accounting.Domain.Entities;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Repositories
{
    internal class ClientSubscriptionRepository : IClientSubscriptionRepository
    {
        private readonly AppDbContext _dbContext;

        public ClientSubscriptionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClientSubscription?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _dbContext.ClientSubscriptions.FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task AddAsync(ClientSubscription subscription, CancellationToken ct)
            => await _dbContext.ClientSubscriptions.AddAsync(subscription, ct);

        public void Remove(ClientSubscription subscription)
            => _dbContext.ClientSubscriptions.Remove(subscription);

        public IQueryable<ClientSubscription> Query()
            => _dbContext.ClientSubscriptions.AsQueryable();
    }
}
