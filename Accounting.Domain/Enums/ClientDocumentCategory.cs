using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Domain.Enums
{
    public enum ClientDocumentCategory
    {
        TaxReport = 0,
        Invoice = 1,
        Act = 2,
        Contract = 3,
        BankStatement = 4,
        Receipt = 5,
        Other = 6
    }
}
