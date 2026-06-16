namespace Accounting.Api.Contracts.Identity
{
    public sealed record UpdateUserRequest(
    string FullName,
    string Email,
    string? TaxId,
    bool IsActive
);
}
