using Accounting.Application.Common;
using Accounting.Application.Features.Users.Common;
using MediatR;

namespace Accounting.Application.Features.Users.GetUsers
{
    /// <param name="Search">
    /// Searches name, email and tax id. The filter has to run on the server:
    /// once the list is paged, an in-browser search would only look at what is
    /// already loaded and would silently miss the rest.
    /// </param>
    public sealed record GetUsersQuery(
        string? Search,
        bool? IsActive,
        int Page,
        int PageSize
    ) : IRequest<PagedResult<UserDto>>;
}
