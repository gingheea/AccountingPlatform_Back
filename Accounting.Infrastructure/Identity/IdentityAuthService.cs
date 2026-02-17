using Accounting.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Identity;

namespace Accounting.Infrastructure.Identity;

public sealed class IdentityAuthService : IIdentityAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public IdentityAuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(UserIdentity User, IList<string> Roles)?> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (!result.Succeeded) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return (new UserIdentity(user.Id, user.Email), roles);
    }
}
