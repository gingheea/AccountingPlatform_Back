using Accounting.Domain.Exceptions;

namespace Accounting.Domain.Entities;

public sealed class ServiceTag
{
    public Guid Id { get; private set; }

    public Guid ServiceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }

    private ServiceTag() { }

    private ServiceTag(Guid id, Guid serviceId, string name, int sortOrder)
    {
        Id = id;
        ServiceId = serviceId;
        SetName(name);
        SortOrder = sortOrder;
    }

    public static ServiceTag Create(Guid serviceId, string name, int sortOrder = 0)
    {
        if (serviceId == Guid.Empty)
            throw new DomainException("Service id is required.");

        return new ServiceTag(Guid.NewGuid(), serviceId, name, sortOrder);
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    private void SetName(string name)
    {
        name = name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tag name is required.");

        if (name.Length > 50)
            throw new DomainException("Tag name max length is 50.");

        Name = name;
    }
}