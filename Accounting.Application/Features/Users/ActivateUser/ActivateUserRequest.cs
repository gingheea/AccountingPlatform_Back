using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.ActivateUser
{
    public sealed record ActivateUserRequest(Guid Id) : IRequest;
}
