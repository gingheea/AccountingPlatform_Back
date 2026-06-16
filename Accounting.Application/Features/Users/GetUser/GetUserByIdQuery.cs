using Accounting.Application.Features.Users.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.GetUser
{
    public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;
}
