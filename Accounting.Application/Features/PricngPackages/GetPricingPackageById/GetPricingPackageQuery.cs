using Accounting.Application.Features.PricingPackages.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Application.Features.PricngPackages.GetPricingPackageById
{
    public sealed record GetPricingPackageQuery(Guid Id) : IRequest<PricingPackageDto>;
}
