namespace Accounting.Application.Abstractions.Auth;

public interface IIdentityAuthService
{
    Task<(UserIdentity User, IList<string> Roles)?> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken ct = default);
}
