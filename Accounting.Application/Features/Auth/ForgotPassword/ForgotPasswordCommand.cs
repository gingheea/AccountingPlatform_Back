using MediatR;

namespace Accounting.Application.Features.Auth.ForgotPassword
{
    public sealed record ForgotPasswordCommand(string Email) : IRequest;
}
