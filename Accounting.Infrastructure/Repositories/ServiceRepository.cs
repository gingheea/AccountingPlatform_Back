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
    internal class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _dbContext;

        public ServiceRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public Task AddAsync(Service service, CancellationToken ct)
        => _dbContext.Services.AddAsync(service, ct).AsTask();

        public Task<Service?> GetByIdAsync(Guid id, CancellationToken ct)
            => _dbContext.Services.FirstOrDefaultAsync(s => s.Id == id, ct);

        public IQueryable<Service> Query() => _dbContext.Services.AsQueryable();

        public void Remove(Service service)
        {
            _dbContext.Services.Remove(service);
        }
    }
}
