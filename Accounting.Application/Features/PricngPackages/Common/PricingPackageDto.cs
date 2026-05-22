namespace Accounting.Application.Features.PricingPackages.Common;

public sealed record PricingPackageDto(
    Guid Id,
    string Name,
    string? Badge,
    string? Description,
    decimal Price,
    string? PriceLabel,
    string? PeriodLabel,
    bool IsRecommended,
    bool IsActive,
    IReadOnlyCollection<string> Features,
    int SortOrder
);