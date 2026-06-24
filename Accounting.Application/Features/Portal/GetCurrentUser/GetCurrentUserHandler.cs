using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Features.Portal.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Portal.GetCurrentUser
{
    public sealed class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, PortalUserDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserManagementService _userManagementService;

        public GetCurrentUserHandler(
            ICurrentUserService currentUserService,
            IUserManagementService userManagementService)
        {
            _currentUserService = currentUserService;
            _userManagementService = userManagementService;
        }

        public async Task<PortalUserDto> Handle(GetCurrentUserQuery request, CancellationToken ct)
        {
            var userId = _currentUserService.UserId;

            if (userId is null)
                throw new UnauthorizedAccessException("Current user is not authenticated.");

            var user = await _userManagementService.GetPortalUserByIdAsync(userId.Value, ct);

            if (user is null)
                throw new UnauthorizedAccessException("Current user is not active or does not exist.");

            return user;
        }
    }
}
