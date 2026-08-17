using Accounting.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Persistence
{
    public interface IClientSubscriptionRepository
    {
        Task<ClientSubscription?> GetByIdAsync(Guid id, CancellationToken ct);

        Task AddAsync(ClientSubscription subscription, CancellationToken ct);

        void Remove(ClientSubscription subscription);

        IQueryable<ClientSubscription> Query();
    }
}
