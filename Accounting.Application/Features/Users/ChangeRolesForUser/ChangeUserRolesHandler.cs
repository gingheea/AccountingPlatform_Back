using Accounting.Application.Abstractions.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.ChangeRolesForUser
{
    public sealed class ChangeUserRolesHandler : IRequestHandler<ChangeUserRolesRequest>
    {
        private readonly IUserManagementService _userManagementService;
        public ChangeUserRolesHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService ?? throw new ArgumentNullException(nameof(userManagementService));
        }

        public async Task Handle(ChangeUserRolesRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManagementService.GetByIdAsync(request.Id, cancellationToken);
            if (user is null)
                throw new InvalidOperationException($"User with id {request.Id} not found.");


            await _userManagementService.ChangeRolesAsync(request.Id, request.Roles, cancellationToken);
        }
    }
}
  