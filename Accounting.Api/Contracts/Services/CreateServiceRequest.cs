namespace Accounting.Api.Contracts.Services;

public sealed record CreateServiceRequest(
    string Name,
    string? Description,
    decimal Price,
    int SortOrder
);