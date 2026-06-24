using Accounting.Application.Features.Portal.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Portal.GetCurrentUser
{
    public sealed record GetCurrentUserQuery : IRequest<PortalUserDto>;
}
