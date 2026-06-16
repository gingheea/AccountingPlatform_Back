using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Features.Users.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.GetUsers
{
    public sealed class GetUsersHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
    {
        private readonly IUserManagementService _userManagementService;

        public GetUsersHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return await _userManagementService.ListAsync(cancellationToken);
        }
    }
}
