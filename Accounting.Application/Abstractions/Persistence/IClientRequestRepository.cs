using Accounting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Persistence
{
    public interface IClientRequestRepository
    {
        Task<ClientRequest?> GetByIdAsync(Guid id, CancellationToken ct);

        Task AddAsync(ClientRequest clientRequest, CancellationToken ct);

        Task<IReadOnlyList<ClientRequest>> ListByUserIdAsync(Guid userId, CancellationToken ct);

        void Remove(ClientRequest clientRequest);

        IQueryable<ClientRequest> Query();
    }
}
