using System.Security.Cryptography.X509Certificates;

namespace Accounting.Api.Contracts.PricingPackagaes
{
    public sealed record CreatePricingPackageRequest
    (
        string Name,
        string? Badge,
        string? Description,
        decimal Price,
        string? PriceLabel,
        string? PeriodLabel,
        bool? IsRecommended,
        IReadOnlyCollection<string> Features,
        int? SortOrder
    );

}
