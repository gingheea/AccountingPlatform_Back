using Accounting.Domain.Entities;

namespace Accounting.Application.Abstractions.Persistence;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Service service, CancellationToken ct);
    Task DeleteTagsByServiceIdAsync(Guid serviceId, CancellationToken ct);

    Task AddTagsAsync(IEnumerable<ServiceTag> tags, CancellationToken ct);
    void Remove(Service service);

    IQueryable<Service> Query();
}