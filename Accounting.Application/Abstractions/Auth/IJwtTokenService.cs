using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Auth
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(UserIdentity user, IList<string> roles);
    }
}
