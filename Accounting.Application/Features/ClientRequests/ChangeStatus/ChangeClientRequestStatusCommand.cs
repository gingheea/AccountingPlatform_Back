using Accounting.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.ChangeStatus
{
    public sealed record ChangeClientRequestStatusCommand(Guid Id, RequestStatus Status) : IRequest;
}
