using Accounting.Application.Features.Services.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.List
{
    public sealed record GetListServicesQuery : IRequest<IReadOnlyList<ServiceDto>>;
}
