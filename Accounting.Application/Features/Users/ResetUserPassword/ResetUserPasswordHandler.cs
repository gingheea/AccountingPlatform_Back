using Accounting.Application.Abstractions.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.ResetUserPassword
{
    public sealed class ResetUserPasswordHandler : IRequestHandler<ResetUserPasswordRequest>
    {
        private readonly IUserManagementService _userManagementService;
        public ResetUserPasswordHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService ?? throw new ArgumentNullException(nameof(userManagementService));
        }
        public async Task Handle(ResetUserPasswordRequest request, CancellationToken cancellationToken)
        { 
            await _userManagementService.ResetPasswordAsync(request.Id, request.NewPassword, cancellationToken);
        }
    }
}
