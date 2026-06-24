using Accounting.Application.Features.Portal.Common;
using Accounting.Application.Features.Users.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Identity
{
    public interface IUserManagementService
    {
        Task<IReadOnlyList<UserDto>> ListAsync(CancellationToken ct);

        Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<PortalUserDto?> GetPortalUserByIdAsync(Guid id, CancellationToken ct);

        Task<Guid> CreateAsync(
            string fullName,
            string email,
            string password,
            string? taxId,
            IReadOnlyCollection<string> roles,
            CancellationToken ct);

        Task UpdateAsync(
            Guid id,
            string fullName,
            string email,
            string? taxId,
            bool isActive,
            CancellationToken ct);

        Task ActivateAsync(Guid id, CancellationToken ct);

        Task<bool> ExistsAsync(Guid id, CancellationToken ct);

        Task DeactivateAsync(Guid id, CancellationToken ct);

        Task ChangeRolesAsync(
            Guid id,
            IReadOnlyCollection<string> roles,
            CancellationToken ct);

        Task ResetPasswordAsync(
            Guid id,
            string newPassword,
            CancellationToken ct);
    }
}
