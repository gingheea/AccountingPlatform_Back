using Accounting.Domain.Exceptions;

namespace Accounting.Domain.Entities;

public sealed class PricingPackage
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string? Badge { get; private set; }
    public string? Description { get; private set; }

    public decimal Price { get; private set; }
    public string? PriceLabel { get; private set; }
    public string? PeriodLabel { get; private set; }

    public bool IsRecommended { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; }

    public List<string> Features { get; private set; } = new();

    // EF Core needs it
    private PricingPackage() { }

    private PricingPackage(
        Guid id,
        string name,
        decimal price,
        string? priceLabel,
        string? periodLabel,
        string? badge,
        string? description,
        bool isRecommended,
        bool isActive,
        int sortOrder)
    {
        Id = id;
        SetName(name);
        SetPrice(price, priceLabel, periodLabel);
        SetBadge(badge);
        SetDescription(description);
        IsRecommended = isRecommended;
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public static PricingPackage Create(
        string name,
        decimal price,
        string? priceLabel = null,
        string? periodLabel = null,
        string? badge = null,
        string? description = null,
        bool isRecommended = false,
        int sortOrder = 0)
    {
        return new PricingPackage(
            Guid.NewGuid(),
            name,
            price,
            priceLabel,
            periodLabel,
            badge,
            description,
            isRecommended,
            isActive: true,
            sortOrder);
    }

    public void Update(
        string name,
        decimal price,
        string? priceLabel,
        string? periodLabel,
        string? badge,
        string? description,
        bool isRecommended,
        bool isActive,
        int sortOrder)
    {
        SetName(name);
        SetPrice(price, priceLabel, periodLabel);
        SetBadge(badge);
        SetDescription(description);
        IsRecommended = isRecommended;
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public void ReplaceFeatures(IEnumerable<string>? features)
    {
        Features = (features ?? Enumerable.Empty<string>())
            .Where(feature => !string.IsNullOrWhiteSpace(feature))
            .Select(feature => feature.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(12)
            .ToList();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private void SetName(string name)
    {
        name = name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Pricing package name is required.");

        if (name.Length > 200)
            throw new DomainException("Pricing package name max length is 200.");

        Name = name;
    }

    private void SetPrice(decimal price, string? priceLabel, string? periodLabel)
    {
        if (price < 0)
            throw new DomainException("Package price cannot be negative.");

        if (priceLabel is not null && priceLabel.Length > 100)
            throw new DomainException("Price label max length is 100.");

        if (periodLabel is not null && periodLabel.Length > 100)
            throw new DomainException("Period label max length is 100.");

        Price = price;
        PriceLabel = string.IsNullOrWhiteSpace(priceLabel) ? null : priceLabel.Trim();
        PeriodLabel = string.IsNullOrWhiteSpace(periodLabel) ? null : periodLabel.Trim();
    }

    private void SetBadge(string? badge)
    {
        if (badge is not null && badge.Length > 100)
            throw new DomainException("Badge max length is 100.");

        Badge = string.IsNullOrWhiteSpace(badge) ? null : badge.Trim();
    }

    private void SetDescription(string? description)
    {
        if (description is not null && description.Length > 1000)
            throw new DomainException("Description max length is 1000.");

        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}