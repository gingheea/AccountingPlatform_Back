using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.ClientRequests.Complete
{
    public sealed record MarkCompletedCommand(Guid Id) : IRequest; 
}
