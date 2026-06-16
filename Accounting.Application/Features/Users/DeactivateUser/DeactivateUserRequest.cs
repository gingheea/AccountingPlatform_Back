using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.DeactivateUser
{
    public sealed record DeactivateUserRequest(Guid Id) : IRequest;

}
