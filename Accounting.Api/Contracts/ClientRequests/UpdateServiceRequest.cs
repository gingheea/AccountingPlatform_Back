namespace Accounting.Api.Contracts.ClientRequests
{
    public sealed record UpdateServiceRequest(
     string Name,
     string? Description,
     decimal Price,
     bool IsActive,
     int SortOrder
 );
}
