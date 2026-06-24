using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Features.Portal.Common;
using Accounting.Application.Features.Users.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Identity;

public sealed class UserManagementService : IUserManagementService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public UserManagementService(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IReadOnlyList<UserDto>> ListAsync(CancellationToken ct)
    {
        var users = await _userManager.Users
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);

        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserDto(
                user.Id,
                user.FullName,
                user.Email ?? string.Empty,
                user.TaxId,
                user.IsActive,
                roles.ToArray(),
                user.CreatedAt
            ));
        }

        return result;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (user is null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto(
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            user.TaxId,
            user.IsActive,
            roles.ToArray(),
            user.CreatedAt
        );
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        return user is not null;
    }

    public async Task<PortalUserDto?> GetPortalUserByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            return null;

        if (!user.IsActive)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new PortalUserDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            user.TaxId,
            roles.ToArray()
        );
    }

    public async Task<Guid> CreateAsync(
        string fullName,
        string email,
        string password,
        string? taxId,
        IReadOnlyCollection<string> roles,
        CancellationToken ct)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FullName = fullName,
            TaxId = string.IsNullOrWhiteSpace(taxId) ? null : taxId.Trim(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", createResult.Errors.Select(x => x.Description))
            );

        if (roles.Count > 0)
        {
            var roleResult = await _userManager.AddToRolesAsync(user, roles);

            if (!roleResult.Succeeded)
                throw new InvalidOperationException(
                    string.Join("; ", roleResult.Errors.Select(x => x.Description))
                );
        }

        return user.Id;
    }

    public async Task UpdateAsync(
        Guid id,
        string fullName,
        string email,
        string? taxId,
        bool isActive,
        CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            throw new InvalidOperationException("User not found.");

        user.FullName = fullName;
        user.Email = email;
        user.UserName = email;
        user.TaxId = string.IsNullOrWhiteSpace(taxId) ? null : taxId.Trim();
        user.IsActive = isActive;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(x => x.Description))
            );
    }

    public async Task ActivateAsync(Guid id, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            throw new InvalidOperationException("User not found.");

        user.IsActive = true;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(x => x.Description))
            );
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            throw new InvalidOperationException("User not found.");

        user.IsActive = false;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(x => x.Description))
            );
    }

    public async Task ChangeRolesAsync(
        Guid id,
        IReadOnlyCollection<string> roles,
        CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            throw new InvalidOperationException("User not found.");

        var currentRoles = await _userManager.GetRolesAsync(user);

        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (!removeResult.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", removeResult.Errors.Select(x => x.Description))
            );

        if (roles.Count > 0)
        {
            var addResult = await _userManager.AddToRolesAsync(user, roles);

            if (!addResult.Succeeded)
                throw new InvalidOperationException(
                    string.Join("; ", addResult.Errors.Select(x => x.Description))
                );
        }
    }

    public async Task ResetPasswordAsync(
        Guid id,
        string newPassword,
        CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            throw new InvalidOperationException("User not found.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(x => x.Description))
            );
    }
}