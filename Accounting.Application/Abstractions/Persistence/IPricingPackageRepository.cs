using Accounting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Persistence
{
    public interface IPricingPackageRepository
    {
        Task AddAsync(PricingPackage pricingPackage, CancellationToken ct);
        Task<PricingPackage?> GetByIdAsync(Guid id, CancellationToken ct);
        IQueryable<PricingPackage> Query();
        void Remove(PricingPackage pricingPackage);
    }
}
