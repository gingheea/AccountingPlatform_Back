namespace Accounting.Api.Contracts.Services
{
    public sealed record UpdateServiceRequest(
     string Name,
     string? Description,
     decimal Price,
     bool IsActive,
     int SortOrder
 );
}
