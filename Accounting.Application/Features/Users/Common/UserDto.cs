using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Users.Common
{
    public sealed record UserDto(
     Guid Id,
     string FullName,
     string Email,
     string? TaxId,
     bool IsActive,
     IReadOnlyCollection<string> Roles,
     DateTimeOffset CreatedAtUtc
 );
}
