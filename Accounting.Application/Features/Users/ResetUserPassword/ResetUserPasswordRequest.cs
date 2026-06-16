using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.ResetUserPassword
{
    public sealed record ResetUserPasswordRequest(Guid Id, string NewPassword) : IRequest;
}
