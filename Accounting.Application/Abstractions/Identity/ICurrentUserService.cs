using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Identity
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
    }
}
