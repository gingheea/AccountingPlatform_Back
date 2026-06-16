using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.UpdateUser
{
    public sealed class UpdateUserHandler : IRequestHandler<UpdateUserRequest>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserHandler(IUserManagementService userManagementService, IUnitOfWork unitOfWork)
        {
            _userManagementService = userManagementService;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            await _userManagementService.UpdateAsync(
                request.Id,
                request.FullName,
                request.Email,
                request.TaxId,
                request.IsActive,
                cancellationToken
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
