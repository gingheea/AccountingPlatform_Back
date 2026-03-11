using Accounting.Domain.Entities;

namespace Accounting.Application.Abstractions.Persistence;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Service service, CancellationToken ct);
    Task UpdateAsync(Service service, CancellationToken ct);
    void Remove(Service service);

    IQueryable<Service> Query();
}