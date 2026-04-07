using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.Reject
{
    public sealed record MarkRejectedCommand(Guid Id) : IRequest;
}
