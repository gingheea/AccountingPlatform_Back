namespace Accounting.Api.Contracts.Identity
{
    public sealed record CreateUserRequest(
    string FullName,
    string Email,
    string Password,
    string? TaxId,
    IReadOnlyCollection<string> Roles
);
}
