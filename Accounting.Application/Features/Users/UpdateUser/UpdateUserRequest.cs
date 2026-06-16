using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.UpdateUser
{
    public sealed record UpdateUserRequest(
        Guid Id,
        string FullName,
        string Email,
        string? TaxId,
        bool IsActive
    ) : IRequest;
}
