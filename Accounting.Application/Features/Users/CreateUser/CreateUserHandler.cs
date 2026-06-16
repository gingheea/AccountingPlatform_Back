using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Abstractions.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.CreateUser
{
    public sealed class CreateUserHandler : IRequestHandler<CreateUserRequest, Guid>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserHandler(IUserManagementService userManagementService, IUnitOfWork unitOfWork)
        {
            _userManagementService = userManagementService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var userId = await _userManagementService.CreateAsync(request.FullName, request.Email, request.Password, request.TaxId, request.Roles, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return userId;
        }
    }
}
