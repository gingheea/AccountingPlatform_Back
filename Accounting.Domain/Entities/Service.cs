namespace Accounting.Domain.Entities;

public sealed class Service
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; } = 0;

    // EF Core needs it
    private Service() { }

    private Service(Guid id, string name, decimal price, string? description, bool isActive, int sortOrder)
    {
        Id = id;
        SetName(name);
        SetPrice(price);
        SetDescription(description);
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public static Service Create(string name, decimal price, string? description = null, int sortOrder = 0)
        => new(Guid.NewGuid(), name, price, description, isActive: true, sortOrder);

    public void Update(string name, decimal price, string? description, bool isActive, int sortOrder)
    {
        SetName(name);
        SetPrice(price);
        SetDescription(description);
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private void SetName(string name)
    {
        name = name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Service name is required.", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Service name max length is 200.", nameof(name));

        Name = name;
    }

    private void SetPrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        Price = price;
    }

    private void SetDescription(string? description)
    {
        if (description is not null && description.Length > 2000)
            throw new ArgumentException("Description max length is 2000.", nameof(description));

        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}