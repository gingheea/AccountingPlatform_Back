using MediatR;

namespace Accounting.Application.Features.Portal.ChangePassword
{
    public sealed record ChangeOwnPasswordCommand(
        string CurrentPassword,
        string NewPassword
    ) : IRequest;
}
