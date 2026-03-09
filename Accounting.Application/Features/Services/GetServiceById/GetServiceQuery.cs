using Accounting.Application.Features.Services.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.Get
{
    public sealed record GetServiceQuery(Guid Id) : IRequest<ServiceDto>;

}
