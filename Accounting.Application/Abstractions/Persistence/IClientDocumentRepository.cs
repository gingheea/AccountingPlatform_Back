using Accounting.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Persistence
{
    public interface IClientDocumentRepository
    {
        Task<ClientDocument?> GetByIdAsync(Guid id, CancellationToken ct);

        Task AddAsync(ClientDocument document, CancellationToken ct);

        void Remove(ClientDocument document);

        IQueryable<ClientDocument> Query();
    }
}
