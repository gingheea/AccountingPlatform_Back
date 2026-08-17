using MediatR;

namespace Accounting.Application.Features.Auth.ResetPassword
{
    public sealed record ResetPasswordCommand(
        string Email,
        string Token,
        string NewPassword
    ) : IRequest;
}
