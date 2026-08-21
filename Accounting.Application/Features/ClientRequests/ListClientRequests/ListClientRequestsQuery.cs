using Accounting.Application.Common;
using Accounting.Application.Features.ClientRequests.Common;
using MediatR;

namespace Accounting.Application.Features.ClientRequests.ListClientRequests
{
    public sealed record ListClientRequestsQuery(int Page, int PageSize)
        : IRequest<PagedResult<ClientRequestDto>>;
}
