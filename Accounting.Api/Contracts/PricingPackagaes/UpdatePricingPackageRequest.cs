namespace Accounting.Api.Contracts.PricingPackagaes
{
    public sealed record UpdatePricingPackageRequest
    (
        string Name,
        string? Badge,
        string? Description,
        decimal Price,
        string? PriceLabel,
        string? PeriodLabel,
        bool? IsRecommended,
        bool? IsActive,
        IReadOnlyCollection<string> Features,
        int? SortOrder
    );
}
