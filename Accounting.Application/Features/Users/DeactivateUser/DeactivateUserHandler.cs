using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.DeactivateUser
{
    public sealed class DeactivateUserHandler : IRequestHandler<DeactivateUserRequest>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateUserHandler(IUserManagementService userManagementService, IUnitOfWork unitOfWork)
        {
            _userManagementService = userManagementService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeactivateUserRequest request, CancellationToken cancellationToken)
        {
            await _userManagementService.DeactivateAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}   


