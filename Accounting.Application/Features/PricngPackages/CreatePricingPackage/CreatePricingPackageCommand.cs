using Accounting.Application.Abstractions.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.CreatePricingPackage
{
    public sealed record CreatePricingPackageCommand(string Name,
        string? Badge,
        string? Description,
        decimal Price,
        string? PriceLabel,
        string? PeriodLabel,
        bool IsRecommended,
        IReadOnlyCollection<string> Features,
        int SortOrder) : IRequest<Guid>, IPricingPackageCommand;
}
