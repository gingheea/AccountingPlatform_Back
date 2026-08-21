using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Common;
using Accounting.Application.Common.Errors;
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

    public async Task<PagedResult<UserDto>> ListAsync(
        string? search,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        page = Pagination.NormalizePage(page);
        pageSize = Pagination.NormalizePageSize(pageSize);

        var query = _userManager.Users.AsNoTracking();

        if (isActive is not null)
            query = query.Where(x => x.IsActive == isActive.Value);

        search = search?.Trim();

        if (!string.IsNullOrWhiteSpace(search))
        {
            // ILike is Postgres' own case-insensitive match.
            // EF.Functions.Like with ToLower() would do the same but force the database
            // to walk every row applying ToLower to each one.
            var pattern = $"%{search}%";

            query = query.Where(x =>
                EF.Functions.ILike(x.FullName ?? string.Empty, pattern) ||
                EF.Functions.ILike(x.Email ?? string.Empty, pattern) ||
                EF.Functions.ILike(x.TaxId ?? string.Empty, pattern));
        }

        var total = await query.CountAsync(ct);

        var users = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var result = new List<UserDto>(users.Count);

        // Identity can only return roles one user at a time, so this is a query per
        // user. It used to walk every user in the system; now it only covers the
        // ones on the current page.
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

        return new PagedResult<UserDto>(result, total);
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

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            throw new NotFoundException($"User {id} was not found.");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            throw new BadRequestException(
                string.Join("; ", result.Errors.Select(x => x.Description)));
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

    public async Task ChangeOwnPasswordAsync(
        Guid id,
        string currentPassword,
        string newPassword,
        CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
            throw new InvalidOperationException("User not found.");

        // ChangePasswordAsync verifies the old password itself, so there is no
        // separate check here that could drift out of step.
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (result.Succeeded)
            return;

        // Identity writes its errors in English; the most common case is translated
        // so the person immediately understands what went wrong.
        if (result.Errors.Any(x => x.Code == "PasswordMismatch"))
            throw new BadRequestException("Поточний пароль неправильний.");

        throw new BadRequestException(
            string.Join("; ", result.Errors.Select(x => x.Description)));
    }

    public async Task<PasswordResetTicket?> CreatePasswordResetTicketAsync(
        string email,
        CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(email);

        // No such user, or the account is disabled: return null.
        // The caller answers "email sent" regardless: otherwise the recovery form
        // could be used to enumerate which accounts exist.
        if (user is null || !user.IsActive)
            return null;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return new PasswordResetTicket(user.Email!, user.FullName ?? string.Empty, token);
    }

    public async Task ResetPasswordWithTokenAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || !user.IsActive)
            throw new BadRequestException("Посилання недійсне або застаріле.");

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (result.Succeeded)
            return;

        // "Invalid token" is handled separately: the link may have been used twice
        // or expired, which is not the same as a weak password.
        if (result.Errors.Any(x => x.Code == "InvalidToken"))
            throw new BadRequestException(
                "Посилання недійсне або застаріле. Запросіть відновлення ще раз.");

        throw new BadRequestException(
            string.Join("; ", result.Errors.Select(x => x.Description)));
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