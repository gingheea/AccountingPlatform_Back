using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Features.Users.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.GetUser
{
    public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserManagementService _userManagementService;

        public GetUserByIdHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }
        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManagementService.GetByIdAsync(request.Id, cancellationToken);

            if (user == null) 
            {
                throw new KeyNotFoundException($"User with ID {request.Id} not found.");
            }

            return user;
        }
    }
}
