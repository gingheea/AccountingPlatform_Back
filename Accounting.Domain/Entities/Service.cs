using Accounting.Domain.Exceptions;
using System.Xml.Linq;

namespace Accounting.Domain.Entities;

public sealed class Service
{
    private readonly List<ServiceTag> _tags = new();

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public decimal Price { get; private set; }
    public string? PriceLabel { get; private set; }

    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; } = 0;

    public IReadOnlyCollection<ServiceTag> Tags => _tags.AsReadOnly();
    // EF Core needs it
    private Service() { }

    private Service(Guid id, string name, decimal price, string? priceLabel, string? description, bool isActive, int sortOrder)
    {
        Id = id;
        SetName(name);
        SetPrice(price, priceLabel);
        SetDescription(description);
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public static Service Create(string name, decimal price, string? priceLabel, string? description = null, int sortOrder = 0)
        => new(Guid.NewGuid(), name, price, priceLabel, description, isActive: true, sortOrder);

    public void Update(string name, decimal price, string? priceLabel, string? description, bool isActive, int sortOrder)
    {
        SetName(name);
        SetPrice(price, priceLabel);
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
            throw new DomainException("Service name is required.");

        if (name.Length > 200)
            throw new DomainException("Service name max length is 200.");

        Name = name;
    }

    private void SetPrice(decimal price, string? priceLabel)
    {
        if (price < 0)
            throw new DomainException("Price cannot be negative.");

        if (priceLabel is not null && priceLabel.Length > 100)
            throw new DomainException("Price label max length is 100.");

        Price = price;
        PriceLabel = string.IsNullOrWhiteSpace(priceLabel) ? null : priceLabel.Trim();
    }
    public void ReplaceTags(IEnumerable<string> tags)
    {
        _tags.Clear();

        var normalizedTags = tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(8)
            .ToList();

        for (var i = 0; i < normalizedTags.Count; i++)
        {
            _tags.Add(ServiceTag.Create(Id, normalizedTags[i], i));
        }
    }

    private void SetDescription(string? description)
    {
        if (description is not null && description.Length > 2000)
            throw new DomainException("Description max length is 2000.");

        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}