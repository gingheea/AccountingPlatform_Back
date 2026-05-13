namespace Accounting.Api.Contracts.Services
{
    public sealed record UpdateServiceRequest(
     string Name,
     string? Description,
     decimal Price,
     string? PriceLabel,
     bool IsActive,
     int SortOrder,
     IReadOnlyCollection<string> Tags
 );
}
