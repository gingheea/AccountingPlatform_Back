using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Abstractions.Persistence
{
    public interface IPricingPackageCommand
    {
        string Name { get; }
        string? Badge { get; }
        string? Description { get; }
        decimal Price { get; }
        string? PriceLabel { get; }
        string? PeriodLabel { get; }
        bool IsRecommended { get; }
        IReadOnlyCollection<string> Features { get; }
        int SortOrder { get; }
    }
}
