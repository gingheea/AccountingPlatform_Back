using Accounting.Application.Abstractions.Persistence;
using Accounting.Domain.Entities;
using Accounting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Infrastructure.Repositories
{
    internal class ClientRequestRepository : IClientRequestRepository
    {
        private readonly AppDbContext _dbContext;

        public ClientRequestRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(ClientRequest clientRequest, CancellationToken ct)
        => await _dbContext.ClientRequests.AddAsync(clientRequest, ct);

        public async Task<ClientRequest?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _dbContext.ClientRequests.FirstOrDefaultAsync(cr => cr.Id == id, ct);

        public IQueryable<ClientRequest> Query()
            => _dbContext.ClientRequests.AsQueryable();


        public async Task<IReadOnlyList<ClientRequest>> ListByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _dbContext.ClientRequests
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(ct);
        }

        public void Remove(ClientRequest clientRequest)
        {
            throw new NotImplementedException();
        }
    }
}
