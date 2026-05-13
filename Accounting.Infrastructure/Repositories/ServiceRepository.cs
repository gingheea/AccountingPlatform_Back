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

        public async Task AddAsync(Service service, CancellationToken ct)
        => await _dbContext.Services.AddAsync(service, ct);

        public async Task<Service?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task DeleteTagsByServiceIdAsync(Guid serviceId, CancellationToken ct)
            => await _dbContext.Set<ServiceTag>()
                .Where(t => t.ServiceId == serviceId)
                .ExecuteDeleteAsync(ct);

        public async Task AddTagsAsync(IEnumerable<ServiceTag> tags, CancellationToken ct)
        {
            await _dbContext.ServiceTags.AddRangeAsync(tags, ct);
        }

        public IQueryable<Service> Query() 
            => _dbContext.Services.AsQueryable();

        public void Remove(Service service)
        {
            _dbContext.Services.Remove(service);
        }
    }
}
