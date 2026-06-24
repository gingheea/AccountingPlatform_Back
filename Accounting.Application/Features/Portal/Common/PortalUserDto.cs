using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.Portal.Common
{
    public sealed record PortalUserDto(
      Guid Id,
      string Email,
      string? FullName,
      string? TaxId,
      IReadOnlyCollection<string> Roles
  );
}
