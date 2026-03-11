namespace Accounting.Api.Contracts.ClientRequests;

public sealed record CreateServiceRequest(
    string Name,
    string? Description,
    decimal Price,
    int SortOrder
);