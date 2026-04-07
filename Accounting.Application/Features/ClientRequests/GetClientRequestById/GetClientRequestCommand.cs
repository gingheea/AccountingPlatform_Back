using Accounting.Application.Features.ClientRequests.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.GetClientRequest
{
    public sealed record GetClientRequestQuery(Guid Id) : IRequest<ClientRequestDto>;
}
