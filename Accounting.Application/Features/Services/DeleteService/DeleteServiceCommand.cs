using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.DeleteService
{
    public sealed record DeleteServiceCommand(Guid Id) : IRequest;
}
