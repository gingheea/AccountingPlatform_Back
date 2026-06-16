namespace Accounting.Api.Contracts.Identity
{
    public sealed record ChangeUserRolesRequest(
     IReadOnlyCollection<string> Roles
 );
}
