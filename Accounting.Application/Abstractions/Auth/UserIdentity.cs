using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Auth
{
    public sealed record UserIdentity(Guid Id, string? Email);
}
