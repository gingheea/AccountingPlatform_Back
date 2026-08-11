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
    internal class ClientDocumentRepository : IClientDocumentRepository
    {
        private readonly AppDbContext _dbContext;

        public ClientDocumentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClientDocument?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _dbContext.ClientDocuments.FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task AddAsync(ClientDocument document, CancellationToken ct)
            => await _dbContext.ClientDocuments.AddAsync(document, ct);

        public void Remove(ClientDocument document)
            => _dbContext.ClientDocuments.Remove(document);

        public IQueryable<ClientDocument> Query()
            => _dbContext.ClientDocuments.AsQueryable();
    }
}
