using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.ActivateUser
{
    public sealed class ActivateUserHandler : IRequestHandler<ActivateUserRequest>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IUnitOfWork _unitOfWork;
        public ActivateUserHandler(IUserManagementService userManagementService, IUnitOfWork unitOfWork)
        {
            _userManagementService = userManagementService;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(ActivateUserRequest request, CancellationToken cancellationToken)
        {
            await _userManagementService.ActivateAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
