using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Services.ActivateService
{
    public sealed record ActivateCommand(Guid id) : IRequest;
}
