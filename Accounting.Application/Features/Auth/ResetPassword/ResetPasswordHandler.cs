using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Common.Errors;
using MediatR;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Auth.ResetPassword
{
    public sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IUserManagementService _userManagementService;

        public ResetPasswordHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            // Розкодовуємо назад те, що для посилання кодували у безпечний вигляд.
            var token = Base64UrlDecode(request.Token);

            await _userManagementService.ResetPasswordWithTokenAsync(
                request.Email.Trim(), token, request.NewPassword, ct);
        }

        private static string Base64UrlDecode(string value)
        {
            var normalized = value.Replace('-', '+').Replace('_', '/');

            // При кодуванні відкидали «=» у кінці — повертаємо їх назад,
            // інакше Base64 не розбереться з довжиною.
            normalized = (normalized.Length % 4) switch
            {
                2 => normalized + "==",
                3 => normalized + "=",
                _ => normalized
            };

            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(normalized));
            }
            catch (FormatException)
            {
                throw new BadRequestException("Посилання недійсне або застаріле.");
            }
        }
    }
}
