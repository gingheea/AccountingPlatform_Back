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
            // Id беремо з токена, а не з тіла запиту: інакше будь-хто міг би
            // передати чужий і змінити не свій пароль.
            var userId = _currentUserService.UserId
                         ?? throw new UnauthorizedAccessException("Current user is not authenticated.");

            await _userManagementService.ChangeOwnPasswordAsync(
                userId, request.CurrentPassword, request.NewPassword, ct);
        }
    }
}
