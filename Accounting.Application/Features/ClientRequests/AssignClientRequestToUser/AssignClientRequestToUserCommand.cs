using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.AssignClientRequestToUser
{
    public sealed record AssignClientRequestToUserCommand(
    Guid ClientRequestId,
    Guid UserId
) : IRequest;
}
