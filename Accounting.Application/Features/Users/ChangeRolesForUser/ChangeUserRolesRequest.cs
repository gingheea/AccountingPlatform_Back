using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.ChangeRolesForUser
{
    public sealed record ChangeUserRolesRequest(Guid Id, IReadOnlyCollection<string> Roles) : IRequest;
}
