using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.UnassignClientRequestUser
{
    public sealed record UnassignClientRequestUserCommand(
      Guid ClientRequestId
  ) : IRequest;
}
