using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.CreateUser
{
    public sealed record CreateUserRequest(
    string FullName,
    string Email,
    string Password,
    string? TaxId,
    IReadOnlyCollection<string> Roles) : IRequest<Guid>;
}
