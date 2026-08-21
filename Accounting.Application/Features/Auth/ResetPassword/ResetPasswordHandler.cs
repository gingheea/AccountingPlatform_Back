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
            // Decode back what was URL-safe encoded for the link.
            var token = Base64UrlDecode(request.Token);

            await _userManagementService.ResetPasswordWithTokenAsync(
                request.Email.Trim(), token, request.NewPassword, ct);
        }

        private static string Base64UrlDecode(string value)
        {
            var normalized = value.Replace('-', '+').Replace('_', '/');

            // Encoding dropped the trailing "=" padding: put it back,
            // otherwise Base64 cannot make sense of the length.
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
