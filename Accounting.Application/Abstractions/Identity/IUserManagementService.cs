using Accounting.Application.Common;
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
        Task<PagedResult<UserDto>> ListAsync(
            string? search,
            bool? isActive,
            int page,
            int pageSize,
            CancellationToken ct);

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

        /// <summary>
        /// Deletes the account for good. Related documents, engagements and the
        /// testimonial go with it: the database relationships take care of that.
        /// </summary>
        Task DeleteAsync(Guid id, CancellationToken ct);

        Task ChangeRolesAsync(
            Guid id,
            IReadOnlyCollection<string> roles,
            CancellationToken ct);

        /// <summary>Changing one's own password: requires confirming the old one.</summary>
        Task ChangeOwnPasswordAsync(
            Guid id,
            string currentPassword,
            string newPassword,
            CancellationToken ct);

        /// <summary>
        /// Prepares a one-time password reset code. Returns null when no such address
        /// exists, so the caller does not have to distinguish the cases and
        /// accidentally reveal whether an account exists.
        /// </summary>
        Task<PasswordResetTicket?> CreatePasswordResetTicketAsync(string email, CancellationToken ct);

        Task ResetPasswordWithTokenAsync(
            string email,
            string token,
            string newPassword,
            CancellationToken ct);

        Task ResetPasswordAsync(
            Guid id,
            string newPassword,
            CancellationToken ct);
    }
}
