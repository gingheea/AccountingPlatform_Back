using Accounting.Application.Features.Users.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.GetUsers
{
    public sealed record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;
}
