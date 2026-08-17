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

        /// <summary>Зміна власного пароля: потребує підтвердження старого.</summary>
        Task ChangeOwnPasswordAsync(
            Guid id,
            string currentPassword,
            string newPassword,
            CancellationToken ct);

        /// <summary>
        /// Готує одноразовий код для відновлення пароля. Повертає null, якщо
        /// такої пошти немає — щоб той, хто викликає, не мусив розрізняти
        /// випадки й ненароком видати, чи існує акаунт.
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
