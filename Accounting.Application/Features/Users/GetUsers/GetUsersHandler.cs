using Accounting.Application.Abstractions.Identity;
using Accounting.Application.Common;
using Accounting.Application.Features.Users.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.GetUsers
{
    public sealed class GetUsersHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
    {
        private readonly IUserManagementService _userManagementService;

        public GetUsersHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
        {
            return await _userManagementService.ListAsync(
                request.Search,
                request.IsActive,
                request.Page,
                request.PageSize,
                ct);
        }
    }
}
