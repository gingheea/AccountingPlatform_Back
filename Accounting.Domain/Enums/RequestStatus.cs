using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Domain.Enums
{
    public enum RequestStatus
    {
        New = 1,
        InProgress = 2,
        WaitingForClient =3,
        Completed = 4,
        Rejected = 5
    }
}
