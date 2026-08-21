using Accounting.Application.Common;
using Accounting.Application.Features.ClientRequests.Common;
using MediatR;

namespace Accounting.Application.Features.Portal.ListClientRequests
{
    public sealed record ListMyClientRequestsQuery(int Page, int PageSize)
        : IRequest<PagedResult<ClientRequestDto>>;
}
