using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Domain.Enums
{
    public enum ClientDocumentStatus
    {
        Uploaded = 0,
        InReview = 1,
        Approved = 2,
        Rejected = 3,
        Archived = 4
    }
}
