using Accounting.Application.Features.ClientRequests.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Portal.ListClientRequests
{
    public sealed record ListMyClientRequestsQuery
    : IRequest<IReadOnlyList<ClientRequestDto>>;
}
