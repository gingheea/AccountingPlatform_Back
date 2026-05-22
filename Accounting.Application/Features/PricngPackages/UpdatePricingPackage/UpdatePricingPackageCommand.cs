using Accounting.Application.Abstractions.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.UpdatePricingPackage
{
    public sealed record UpdatePricingPackageCommand
        (
            Guid Id,
            string Name,
            string? Badge,
            string? Description,
            decimal Price,
            string? PriceLabel,
            string? PeriodLabel,
            bool IsRecommended,
            bool IsActive,
            IReadOnlyCollection<string> Features,
            int SortOrder
        ) : IRequest , IPricingPackageCommand;
}
