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
    internal sealed class PricingPackageRepository : IPricingPackageRepository
    {
        private readonly AppDbContext _dbContext;
        public PricingPackageRepository(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(PricingPackage pricingPackage, CancellationToken ct)
        => await _dbContext.PricingPackages.AddAsync(pricingPackage, ct);

        public async Task<PricingPackage?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _dbContext.PricingPackages.FirstOrDefaultAsync(pp=> pp.Id == id, ct);

        public IQueryable<PricingPackage> Query()
        => _dbContext.PricingPackages.AsQueryable();

        public void Remove(PricingPackage pricingPackage)
        {
           _dbContext.PricingPackages.Remove(pricingPackage);  
        }
    }
}
