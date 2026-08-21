using Accounting.Application.Abstractions.Identity;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Portal.ChangePassword
{
    public sealed class ChangeOwnPasswordHandler : IRequestHandler<ChangeOwnPasswordCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserManagementService _userManagementService;

        public ChangeOwnPasswordHandler(
            ICurrentUserService currentUserService,
            IUserManagementService userManagementService)
        {
            _currentUserService = currentUserService;
            _userManagementService = userManagementService;
        }

        public async Task Handle(ChangeOwnPasswordCommand request, CancellationToken ct)
        {
            // The id comes from the token, not from the request body: otherwise anyone
            // could pass somebody else's and change a password that is not theirs.
            var userId = _currentUserService.UserId
                         ?? throw new UnauthorizedAccessException("Current user is not authenticated.");

            await _userManagementService.ChangeOwnPasswordAsync(
                userId, request.CurrentPassword, request.NewPassword, ct);
        }
    }
}
