using Accounting.Application.Features.ClientRequests.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.ListClientRequests
{
   public sealed record ListClientRequestsQuery : IRequest<IReadOnlyList<ClientRequestDto>>;
}
